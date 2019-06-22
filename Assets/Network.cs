using System;
using System.Collections.Generic;

using TargetShooting;
using TargetShooting.NeuralNetwork;

namespace TargetShooting.NeuralNetwork
{
	//Provides core network functionality (iteration, (de)serialization, et cetera) and contains the neuron collection
	class Network
	{
		public List<Neuron> neurons;
		int nextID;
		List<int> inputNeurons;
		List<int> outputNeurons;
		List<int> hiddenNeurons;
		
		public Network()
		{
			nextID = 0;
			neurons = new List<Neuron>();
			inputNeurons = new List<int>();
			outputNeurons = new List<int>();
			hiddenNeurons = new List<int>();
		}
		
		//Connectivity is the number of synapses on each hidden neuron; 0 means every hidden neuron connects to every other hidden neuron
		public Network(int inputWidth, int outputWidth, int hiddenCount, int connectivity = 0)
		{
			nextID = 0;
			neurons = new List<Neuron>();
			inputNeurons = new List<int>();
			outputNeurons = new List<int>();
			hiddenNeurons = new List<int>();
			
			for(int i = 0; i < inputWidth; i++)
			{
				AddInputNeuron();
			}
			
			if(hiddenCount == 0) //perceptron
			{
				for(int i = 0; i < outputWidth; i++)
				{
					AddOutputNeuron(inputNeurons);
				}
				return;
			}
			
			for(int i = 0; i < outputWidth; i++)
			{
				AddOutputNeuron();
			}
			
			for(int i = 0; i < hiddenCount; i++)
			{
				AddHiddenNeuron();
			}
			
			List<Tuple<int, int, float>> incidence = new List<Tuple<int, int, float>>();	//represents <from, to, weight>
			for(int i = 0; i < hiddenNeurons.Count; i++)
			{
				if(connectivity == 0) //all-to-all case
				{
					for(int j = 0; j < hiddenNeurons.Count; j++)
					{
						incidence.Add(new Tuple<int, int, float>(j, i, (float)Util.rng.NextDouble()));
					}
					continue;
				}
				for(int j = 0; j < connectivity; j++)
				{
					incidence.Add(new Tuple<int, int, float>(Util.rng.Next(0,hiddenNeurons.Count), i, (float)Util.rng.NextDouble()));
				}
			}
			
			//tie hidden neuron ball into the input and output layers
			for(int i = 0; i < hiddenNeurons.Count; i++)
			{
				for(int j = 0; j < inputNeurons.Count; j++)
				{
					incidence.Add(new Tuple<int, int, float>(j, i, (float)Util.rng.NextDouble()));
				}
				for(int j = 0; j < outputNeurons.Count; j++)
				{
					incidence.Add(new Tuple<int, int, float>(i, j, (float)Util.rng.NextDouble()));
				}
			}
			
			IncidenceToNetwork(incidence);
		}
		
		public void Iterate()
		{
			for(int i = 0; i < neurons.Count; i++)
			{
				neurons[i].ComputeState();
			}
		}
		
		public void SetInputs(float[] inputs)
		{
			if(inputs.Length == inputNeurons.Count)
			{
				for(int i = 0; i < inputs.Length; i++)
				{
					neurons[inputNeurons[i]].state = inputs[i];
				}
			}
		}
		
		public float[] GetOutputs()
		{
			float[] outputs = new float[outputNeurons.Count];
			for(int i = 0; i < outputNeurons.Count; i++)
			{
				outputs[i] = neurons[outputNeurons[i]].state;
			}
			return outputs;
		}
		
		/*Serialization Specification
			0: Number of inputs (I)
			1: Number of outputs (O)
			2: Number of hiddens (H)
			Next (H+I+O)*3:
				0: Origin of synapse
				1: Target of synapse
				2: Weight of synapse (convert back to float!)
		*/

		public uint[] Serialize()
		{
			CompactIOLayers();
			List<Tuple<int, int, float>> incidence = NetworkToIncidence();
			uint[] output = new uint[incidence.Count*3 + 3];
			output[0] = (uint)inputNeurons.Count;
			output[1] = (uint)outputNeurons.Count;
			output[2] = (uint)hiddenNeurons.Count;
			for(int i = 1; i <= incidence.Count; i++)
			{
				output[i*3  ] = BitConverter.ToUInt32(BitConverter.GetBytes(incidence[i].Item1),0);
				output[i*3+1] = BitConverter.ToUInt32(BitConverter.GetBytes(incidence[i].Item2),0);
				output[i*3+2] = BitConverter.ToUInt32(BitConverter.GetBytes(incidence[i].Item3),0);
			}
			return output;
		}
		
		//*
		public static Network Deserialize(uint[] flat)
		{
			Network output = new Network();
			List<Tuple<int, int, float>> incidence = new List<Tuple<int, int, float>>();
			for(int i = 0; i < flat[0]; i++)
			{
				output.AddInputNeuron();
			}
			for(int i = 0; i < flat[1]; i++)
			{
				output.AddOutputNeuron();
			}
			for(int i = 0; i < flat[2]; i++)
			{
				output.AddHiddenNeuron();
			}
			
			for(int i = 1; i < flat.Length / 3; i++)
			{
				incidence.Add(new Tuple<int, int, float>((int)flat[i*3], (int)flat[i*3+1], BitConverter.ToSingle(BitConverter.GetBytes(flat[i*3+2]),0)));
			}
			output.IncidenceToNetwork(incidence);
			
			return output;
		}
		//*/
		
		void CompactIOLayers()
		{
			//TODO: Compact neuron collection to all input neurons, all output neurons, all hidden neurons
			//For now: Just check to make sure that condition is true, since that's how we're generating them in the first place.
			//TODO: implement the above check
		}
		
		List<Tuple<int, int, float>> NetworkToIncidence()
		{
			//TODO
			List<Tuple<int, int, float>> incidence = new List<Tuple<int, int, float>>();
			List<Tuple<int, float>> adjacence;
			for(int i = 0; i < neurons.Count; i++)
			{
				adjacence = neurons[i].adjacence;
				for(int j = 0; j < adjacence.Count; j++)
				{
					incidence.Add(new Tuple<int, int, float>(adjacence[j].Item1, i, adjacence[j].Item2));
				}
			}
			return incidence;
		}
		
		void IncidenceToNetwork(List<Tuple<int, int, float>> incidence)
		{
			for(int i = 0; i < incidence.Count; i++)
			{
				neurons[incidence[i].Item2].AddSynapse(incidence[i].Item1, incidence[i].Item3);
			}
		}
		
		void AddNeuron()
		{
			neurons.Add(new Neuron(this));
			nextID++;
		}
		
		void AddInputNeuron()
		{
			inputNeurons.Add(nextID);
			AddNeuron();
		}
		
		void AddOutputNeuron()
		{
			outputNeurons.Add(nextID);
			AddNeuron();
		}
		
		void AddOutputNeuron(List<int> inputs)
		{
			int target = nextID;
			AddOutputNeuron();
			for(int i = 0; i < inputs.Count; i++)
			{
				neurons[target].AddSynapse(inputs[i], (float)Util.rng.NextDouble());
			}
		}
		
		void AddHiddenNeuron()
		{
			hiddenNeurons.Add(nextID);
			AddNeuron();
		}
	}
}
