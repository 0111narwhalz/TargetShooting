using System;
using System.Collections.Generic;

using TargetShooting.NeuralNetwork;

namespace TargetShooting.NeuralNetwork
{
	//Basic component of neural network; contains synapses in an adjacence table and the neuron state.
	class Neuron
	{
		Network parent;
		public List<Tuple<int, float>> adjacence;
		public float state;
		
		public Neuron(Network setParent, List<Tuple<int,float>> setadjacence, float setState = 0)
		{
			parent = setParent;
			adjacence = setadjacence;
			state = setState;
		}
		
		public Neuron(Network setParent)
		{
			parent = setParent;
			adjacence = new List<Tuple<int,float>>();
			state = 0;
		}
		
		public void AddSynapse(int origin, float weight)
		{
			adjacence.Add(new Tuple<int, float>(origin, weight));
		}
		
		public int[] GetAdjacentNodes()
		{
			int[] output = new int[adjacence.Count];
			for(int i = 0; i < adjacence.Count; i++)
			{
				output[i] = adjacence[i].Item1;
			}
			return output;
		}
		
		float Activation(float stimulus)
		{
			float ex = (float)Math.Exp(stimulus);
			float enegx = 1 / ex;
			return (ex - enegx) / (ex + enegx);
		}
		
		public void ComputeState()
		{
			float newState = 0;
			for(int i = 0; i < adjacence.Count; i++)
			{
				newState += parent.neurons[adjacence[i].Item1].state * adjacence[i].Item2;
			}
			state = Activation(newState);
		}
	}
}
