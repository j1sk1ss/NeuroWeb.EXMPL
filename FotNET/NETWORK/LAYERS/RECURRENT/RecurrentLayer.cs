using FotNET.NETWORK.LAYERS.ACTIVATION.ACTIVATION_FUNCTION;
using FotNET.NETWORK.LAYERS.RECURRENT.RECURRENCY_TYPE;
using FotNET.NETWORK.OBJECTS.MATH_OBJECTS;

namespace FotNET.NETWORK.LAYERS.RECURRENT;

public class RecurrentLayer : ILayer {

    public RecurrentLayer(Function function, IRecurrent recurrentType, int size) {
        InputWeights  = new Matrix(1, size);
        HiddenWeights = new Matrix(size, size);
        OutputWeights = new Matrix(size, 1);

        InputWeights.HeInitialization();
        HiddenWeights.HeInitialization();
        OutputWeights.HeInitialization();
        
        HiddenNeurons = new List<double[]>();
        OutputNeurons = new List<double>();
        
        HiddenBias    = new double[size];
        HiddenBias[0] = .01d;
        OutputBias    = .01d;
        
        Function      = function;
        RecurrentType = recurrentType;
    }

    public Matrix InputWeights { get; set; }

    public Matrix HiddenWeights { get; set; }
    
    public Matrix OutputWeights { get; set; }
    
    public double[] HiddenBias { get; }
    
    public double OutputBias { get; set; }
    

    public Function Function { get; }
    public IRecurrent RecurrentType { get; }


    public List<double[]> HiddenNeurons { get; }
    public List<double> OutputNeurons { get; }

    
    public Tensor GetNextLayer(Tensor tensor) => RecurrentType.GetNextLayer(this, tensor);

    public Tensor BackPropagate(Tensor error, double learningRate) => RecurrentType.BackPropagate(this, error, learningRate);

    public Tensor GetValues() => null!;

    public string GetData() => InputWeights.GetValues() + HiddenWeights.GetValues() + OutputWeights.GetValues() 
                               + new Vector(HiddenBias).Print() + " " + OutputBias;
    
    public string LoadData(string data) {
        var position = 0;
        var dataNumbers = data.Split(" ");
        
        for (var x = 0; x < InputWeights.Rows; x++)
            for (var y = 0; y < InputWeights.Columns; y++)
                InputWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);
        
        for (var x = 0; x < HiddenWeights.Rows; x++)
            for (var y = 0; y < HiddenWeights.Columns; y++)
                HiddenWeights.Body[x, y] = double.Parse(dataNumbers[position++]);

        for (var i = 0; i < HiddenBias.Length; i++)
            HiddenBias[i] = double.Parse(dataNumbers[position++]);

        OutputBias = double.Parse(dataNumbers[position++]);
        
        return string.Join(" ", dataNumbers.Skip(position).Select(p => p.ToString()).ToArray());
    }
}