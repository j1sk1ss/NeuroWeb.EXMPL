using FotNET.NETWORK.MATH.OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE.MANY_TO_ONE;

/// <summary>
/// Type on RNN where model takes sequence and return one element
/// </summary>
public class ManyToOne : IRecurrentType {
    public Tensor GetNextLayer(RecurrentLayer layer, Tensor tensor) {
        var sequence = tensor.Flatten();
        
        for (var step = 0; step < sequence.Count; step++) {
            var currentElement = sequence[step];
            var inputNeurons = Matrix.Multiply(new Matrix(new[] { currentElement }), layer.InputWeights);
            
            if (step > 0)
                layer.HiddenNeurons.Add(inputNeurons + Matrix.Multiply(layer.HiddenNeurons[step - 1], layer.HiddenWeights) + new Matrix(layer.HiddenBias).Transpose());
            else
                layer.HiddenNeurons.Add(inputNeurons);

            layer.HiddenNeurons[^1] = layer.Function.Activate(layer.HiddenNeurons[^1]);
            layer.OutputNeurons.Add(Matrix.Multiply(layer.HiddenNeurons[^1], layer.OutputWeights) + layer.OutputBias);
        }

        return new Tensor(layer.OutputNeurons[^1]);
    }

    public Tensor BackPropagate(RecurrentLayer layer, Tensor error, double learningRate) {
        var currentError = error.Flatten()[0];
        var nextHidden = new Matrix(0,0);
        
        var transposedOutputWeights = layer.OutputWeights.Transpose();
        var transposedHiddenWeights = layer.HiddenWeights.Transpose();
        
        for (var step = layer.HiddenNeurons.Count - 1; step >= 0; step--) {
            layer.OutputWeights -= Matrix.Multiply(layer.HiddenNeurons[step].Transpose(),
                new Matrix(new[] { currentError })) * learningRate;
            layer.OutputBias -= currentError * learningRate;
            
            var outputGradient = Matrix.Multiply(new Matrix(new[] { currentError }), transposedOutputWeights);
            if (step != layer.HiddenNeurons.Count - 1) 
                nextHidden = outputGradient + Matrix.Multiply(nextHidden, transposedHiddenWeights);
            else nextHidden = outputGradient;
            
            nextHidden = layer.Function.Derivation(layer.HiddenNeurons[step], layer.HiddenNeurons[step]) * nextHidden;
            if (step > 0) {
                var hiddenWeightGradient = Matrix.Multiply(layer.HiddenNeurons[step - 1].Transpose(), nextHidden);
                layer.HiddenWeights -= hiddenWeightGradient * learningRate;
                for (var bias = 0; bias < layer.HiddenBias.Length; bias++)
                    layer.HiddenBias[bias] -= hiddenWeightGradient.GetAsList().Average() * learningRate;                
            }
            
            layer.InputWeights -= Matrix.Multiply(new Matrix(new[]{layer.InputData.Flatten()[step]}), nextHidden) * learningRate;
        }

        return error;
    }
}