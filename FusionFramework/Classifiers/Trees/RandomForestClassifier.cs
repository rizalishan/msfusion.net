﻿using Accord.MachineLearning.DecisionTrees;
using Accord.Math.Optimization.Losses;
using FusionFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FusionFramework.Classifiers.Trees
{
    public class RandomForestClassifier : IClassifier
    {
        /// <summary>
        /// The Learning algorithm used to train the model.
        /// </summary>
        RandomForestLearning LearningAlgorithm;

        /// <summary>
        /// Model that will be used for classification and training
        /// </summary>
        RandomForest Model;

        /// <summary>
        /// Maximum number of trees.
        /// </summary>
        int NumTrees;

        /// <summary>
        /// Sample propotion for forest
        /// </summary>
        double SamplePropotion;

        /// <summary>
        /// Instantiate Classifier.
        /// </summary>
        /// <param name="numTrees">Maximum number of trees.</param>
        public RandomForestClassifier(int numTrees)
        {
            NumTrees = numTrees;
        }

        /// <summary>
        /// Instantiate Classifier.
        /// </summary>
        /// <param name="numTrees">Maximum number of trees.</param>
        /// <param name="samplePropotion">Sample propotion.</param>
        public RandomForestClassifier(int numTrees, double samplePropotion)
        {
            SamplePropotion = samplePropotion;
        }

        /// <summary>
        /// Perform classification from loaded model and returns the infered class.
        /// </summary>
        /// <param name="vector">The input vector that should be list of double.</param>
        /// <returns>Resultant class as an integer</returns>
        public override int Classify(List<double> vector)
        {
            return Model.Decide(vector.ToArray());
        }

        /// <summary>
        /// Trains the classifier and computes the training error if option provided.
        /// </summary>
        /// <param name="trainingData">The training data that will be used to train classifier.</param>
        /// <param name="trainingLabels">The training labels related to provided training data.</param>
        /// <param name="calculateError">The boolean check to tell if the training error should be calculated.</param>
        public override void Train(List<double[]> trainingData, List<int> trainingLabels, bool calculateError = true)
        {
            LearningAlgorithm = new RandomForestLearning();
            if (NumTrees > 0)
            {
                LearningAlgorithm.NumberOfTrees = NumTrees;
            }

            if (SamplePropotion > 0)
            {
                LearningAlgorithm.SampleRatio = SamplePropotion;
            }

            Model = LearningAlgorithm.Learn(trainingData.ToArray(), trainingLabels.ToArray());
            if(calculateError == true)
            {
                TrainingError = new ZeroOneLoss(trainingLabels.ToArray()).Loss(Model.Decide(trainingData.ToArray()));
            }
        }

        /// <summary>
        /// Trains the classifier and computes the training error if option provided.
        /// </summary>
        /// <param name="trainingData">The training data that will be used to train the classifier.</param>
        /// <param name="trainingLabels">The training labels related to provided training data.</param>
        /// <param name="testingData">The testing data that will be used to test the classifier.</param>
        /// <param name="testingLabels">The testing labels related to provided test the output of the classifier.</param>
        public override void Train(List<double[]> trainingData, List<int> trainingLabels, List<double[]> testingData, List<int> testingLabels)
        {
            Train(trainingData, trainingLabels);
            CalculateTrainingError(testingData, testingLabels);
        }

        /// <summary>
        /// Loads the trained model.
        /// </summary>
        /// <param name="path">The location from where to load the trained model.</param>
        public override void Load(string path)
        {
            Model = Accord.IO.Serializer.Load<RandomForest>(path);
        }

        /// <summary>
        /// Saves the trained model.
        /// </summary>
        /// <param name="path">The location where to save the trained model.</param>
        public override void Save(string path)
        {
            Accord.IO.Serializer.Save<RandomForest>(Model, path);
        }

        /// <summary>
        /// Calculates error after training the model.
        /// </summary>
        /// <param name="testData">The test data that would be used to calculate error.</param>
        /// <param name="testOutput">The test labels that would be used to calculate error.</param>
        public override void CalculateTrainingError(List<double[]> testData, List<int> testOutput)
        {
            TrainingError = new ZeroOneLoss(testData.ToArray()).Loss(Model.Decide(testData.ToArray()));
        }
    }
}
