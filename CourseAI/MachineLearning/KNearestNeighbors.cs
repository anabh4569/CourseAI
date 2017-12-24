using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearAlgebra;
using CourseAI.Helper;

namespace CourseAI.MachineLearning
{
    sealed class KNearestNeighbors
    {
        /* Supervised vs unsupervised: we always get the features, but do we get the outputs or not? The model we make is
         * either predefined by its labels or unlabeled groups/conclusions that we have to interpret (maybe into groups).
         * 
         * Linear/logistic/polynomial/etc regression is all supervised (we know expected outputs).
         * Clustering/segmentation is unsupervised (model identifies groups just from features, which we might later label).
         * 
         * UL Methods: K-means, gaussian mixtures, hierarchical/spectral clustering, etc
         * SL Methods: Support vector machines, neural networks, decision trees, k-nearest neighbors, naive Bayes, etc
         * 
         * K nearest neighbors: similarly featured examples should have same labels, not model built. Assumption: all inputs
         * are on the same input space/have the same feature count. Euclidian distance used to define nearest neighbors.
         * 
         * Algorithm: Add each training example (x-vector, y in {1, -1}) to the set.
         * Hypothesis(x) = sign(sum(y's of the k nearest neighbors)) -> majority voting; remove sign() for 3+
         * classifications so you can see which result has the highest.
         * Note, once again: this is not a 'model': it's a tool for guessing the label of a new entry based on existing ones.
         * 
         * Where training/testing come in: finding k. Consider a bunch of points around the space; we can create decision
         * boundaries where k-nn will guess one way or the other -> either manually or automatically choose a correct k.
         */

        private KNearestNeighbors() { }
        
        //Outputs must be one-hot vectors (single 0/1's are technically)
        public KNearestNeighbors(TrainingSet data, int k)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            if (k > data.TrainingExamples)
                throw new ArgumentOutOfRangeException();
            this.k = k;
        }

        //Features vector -> guess Vector (different behavior for singular (1 vs -1)/one-hot outputs (another one-hot))
        public Vector Hypothesis(Vector input)
        {
            //Find the k nearest neighbors
            List<Vector> nearest = new List<Vector>((Vector[])data.outputs);
            //If a's closer than b, it's earlier in the list now. nearest[0-k) are input's nearest.
            nearest.Sort(delegate (Vector a, Vector b) { return input.Distance(a).CompareTo(input.Distance(b)); });

            Vector output = new Vector(0f, data.outputs.RowCount);
            for (int i = 0; i < k; i++)
            {
                output += nearest[i];
            }
            //Different behavior; -1/1 float vs one-hot vector
            if (output.Dimension == 1)
            {
                output = new Vector(Math.Sign(output[0]));
            }
            else
            {
                float[] vals = new float[output.Dimension];
                int maxIndex = 0;
                float maxVal = float.NegativeInfinity;
                for (int i = 0; i < output.Dimension; i++)
                {
                    if (output[i] > maxVal)
                    {
                        maxIndex = i;
                        maxVal = output[i];
                    }
                }
                vals[maxIndex] = 1;
                output = new Vector(vals);
            }
            return new Vector(output);
        }

        private TrainingSet data; //Immutable :D, no constraints on outputs' row count because we're gonna allow multiple categories
        private int k; //Want to be close small (1/20 - 1/10 relative to training examples, but only if huge set)

        /* Pros: Simple to implement, works well in practice, no model/parameters to tune, can easily be used on new examples.
         * Cons: Requires large space to store dataset, takes O(examples * features) for each hypothesis, curse of dimensionality,
         * which is that Euclidean distance starts to mean very less as you add more features/dimensions.
         * Applications: cancer tumors, information retrieval
         * 
         * Cost/E_train(f, training) = sum(loss(y_i, x_i) for all in t); Many 'loss' functions like squared error.
         * Here's what we're using: loss(y, f(x)) = 1 if sign(y) != sign(f(x)), 0 otherwise [note, y in {-1, 1}].
         * 
         * Under/overfitting: reducing training and test error, want a good balance between -> medium model complexity
         * Bias: error from erroneous assumptions in the model; Variance: error from sensitivity to small changes in training/test set.
         * Underfitting: high bias, low variance; Overfitting: low bias, high variance.
         * How to avoid overfitting: reduce feature count, model selection, regularization, cross-validating with many test sets.
         * 
         * Regulariztion: error/cost = sum(loss(y_i, f(x_i)) for all i; add C/lambda * R(f) (some regularization term);
         * From other project, this should be thetas to some power, but this isn't a model and has no parameters!
         * Solution: we're not gonna use this on knn (oops) -> perfect for linear/logistic regression though!
         * 
         * Train, validation, test: 60/20/20 split of your data -> training set to learn the model, validation to fine tune model
         * parameters and prevent overfitting, like k in k-nn, and test set is used to assess the performance of the final model
         * and see if overfitting was actually prevented. Note: don't use the test set for anything else (overfit test set too).
         * 
         * K-fold Cross Validation: estimating test error using training data (squeezing most out of training data).
         * Given a learning algorithm A (including its params) and a dataset D, randomly partition D into k equal-size subsets D_1, ..., D_k;
         * For j = 1-k, train A on all D_i, i in {1-k : i != k} to get f_j. Apply f_j to D_j (everything but j was training, j was test).
         * This gives us E^(D_j); average the error over all j.
         * 
         * Confusion matrix (for binary classification): where errors arise. -> true/false positive/negative. TP, TN, FP, FN.
         * Accuracy = % of correct predictions = (TP + TN) / (TP + TN + FP + FN).
         * Precision = % of positive predictions that were correct = TP / (TP + FP)
         * Sensitivity/recall = % of actual positives that were predicted positive = TP / (TP + FN)
         * Specificity = % of actual negatives that were predicted negative = TN / (TN + FP)
         * 
         * Linear (matrix, normal eq)/logistic regression(new convex model for gradient descent) models: already done!
         * 
         * Perceptron: small scale logistic regression -> polarizes items to positive/negatives; only linear terms and seems to
         * blindly update weights by nudging the parameters in the direction of the correct output by each misclassified
         * example's weight vector instead of using gradient descent.
         * 
         * Tree classifiers: root holds all examples, branches specify more information about them (like a species/action chart).
         * Greedy selection of next best feature to branch with; Splitting criteria is 'node purity'. This means you use features
         * to try and polarize the group between positive/negative classifications (until you reach model's yes/no answers).
         * 
         * Note purity (clarification): try to create branches that are as pure as possible -> highest possible % of your
         * output classifications (positive/negative). Note: we're using the C4.5 tree building algorithm.
         * How do we measure this? Entropy(s) = plog(p) + nlog(n); max(Entropy) = most pure; note: course put negatives and told to min;
         * Note: p/n are proportions positive/negative of total; don't consider 0/0's, and p/0 or 0/n have high priority;
         * Generally, for c classes: Entropy(s = node) = sum(-p_i * log(p_i)) over i in {1-c};
         * Information gain(S = current state, A = feature/attribute to branch by) = Entropy(S) - sum((|S_v|/|S|)Entropy(S_v)) for choice v in A's branches.
         * Note: S_v is the state obtained from taking branch v from S, |S| = # of objects at this node -> |S_v|/|S| = v's proportion of S's nodes.
         * Note: making tree builders for specific builders would probably be easy, but for general classes/structs would require
         * extensive knowledge of reflection api (not_me_irl).
         * 
         * 
         */
    }
}
