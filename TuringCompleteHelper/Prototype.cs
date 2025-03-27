using System.Diagnostics;
using TuringCompleteHelper.FixedPointMath;

namespace TuringCompleteHelper;

public class Prototype
{
     public static string Hex(double number) =>
        FixedPointNumberConverter.ConvertDoubleStringToQ16_16(number.ToString("00000.00000000000"));

    public static double FixedPoint(string hex) =>
        double.Parse(FixedPointNumberConverter.ConvertQ16_16ToDoubleString(hex));

    public static string FixedPointStr(string hex) =>
	    FixedPointNumberConverter.ConvertQ16_16ToDoubleString(hex);



    public static double Clamp(double value) => Math.Min(Math.Max(value, FixedPointNumber.MinValue.Value), FixedPointNumber.MaxValue.Value);

    public static double Multiply(double a, double b) => Clamp(a * b);

    public static double Sum(double a, double b) => Clamp(a + b);

    public static double Dot(double[] a, double[] b) => a.Zip(b).Aggregate(0d, (result, x) => Sum(result, Multiply(x.First , x.Second))); 

  

    private static void DoStuff()
    {
	    Console.WriteLine(Hex(Multiply(FixedPoint("926B"), FixedPoint("78"))));
	    double[] weights =
	    [
		    FixedPoint(Hex(00000.30625897338)), FixedPoint(Hex(00000.12179558196)), FixedPoint(Hex(00000.57194485074)), FixedPoint(Hex(1))
	    ];


	    double[] colors = 
	    [
		    FixedPoint("004CAAC6"), FixedPoint("00B37AED"), FixedPoint("003BEC78")
	    ];

	    double[] transposedColors =
	    [
		    FixedPoint("00C6ED78"), FixedPoint("00AA7AEC"), FixedPoint("004CB33B"), FixedPoint(Hex(1))
	    ];

	    double[] splitRed = 
	    [
		    FixedPoint("C6"),
		    FixedPoint("ED"),
		    FixedPoint("78"),
		    FixedPoint(Hex(1))
	    ];
	    
	    Console.WriteLine("Weights:");
	    weights.ToList().ForEach(x=>Console.WriteLine(Hex(x)));
	    splitRed.ToList().ForEach(x=>Console.WriteLine(Hex(x)));
	    
	    double[] splitGreen = 
	    [
		    FixedPoint("AA"),
		    FixedPoint("7A"),
		    FixedPoint("EC"),
		    FixedPoint(Hex(1))
	    ];
	    
	    double[] splitBlue = 
	    [
		    FixedPoint("4C"),
		    FixedPoint("B3"),
		    FixedPoint("3B"),
		    FixedPoint(Hex(1))
	    ];
	    
	    
	    Console.WriteLine();
	    
	    Console.WriteLine(Hex(Dot(weights, splitRed)) );
	    Console.WriteLine(Hex(Dot(weights, splitGreen)));
	    Console.WriteLine(Hex(Dot(weights, splitBlue)));
	    
	    Console.WriteLine(Hex(Dot(weights, splitRed)-FixedPoint(Hex(1))) );
	    Console.WriteLine(Hex(Dot(weights, splitGreen)-FixedPoint(Hex(1))));
	    Console.WriteLine(Hex(Dot(weights, splitBlue)-FixedPoint(Hex(1))));
	    
	    Console.Write("Expected: ");
	    Console.WriteLine(Convert.ToString(10406222, 16).ToUpper());
	    Console.WriteLine(Convert.ToString(6879081, 16).ToUpper());
	    Console.WriteLine();
	    
	    Console.WriteLine($"(0x{Hex(00000.30625897338)} * 0x4CAAC6) + (0x{Hex(00000.12179558196)} * 0xB37AED) + (0x{Hex(00000.57194485074)} * #3BEC78) == #4EC99E");
	    Console.WriteLine($"({FixedPoint(Hex(00000.30625897338))} * 0x4CAAC6) + ({FixedPoint(Hex(00000.12179558196))} * 0xB37AED) + ({FixedPoint(Hex(00000.57194485074))} * #3BEC78) == #4EC99E");
	    return;
	    Console.WriteLine($@"Maximum Value: {FixedPointNumber.MaxValue.Value}");
	    Console.WriteLine($@"Minimum Value: {FixedPointNumber.MinValue.Value}");
	    double mulCheck = Multiply(FixedPoint("01A1EB33"), FixedPoint("FFC5F805"));
	    var hexMul = Hex(mulCheck);
           
	    double mulCheck1 = Multiply(FixedPoint("EFBEEE"), FixedPoint("17784A2"));
	    var hexMul1 = Hex(mulCheck1);
	    Debug.Assert(hexMul1 == "7FFFFFFF");
           
	    DotDotDot();
	    return;

	    //Console.WriteLine(FixedPointNumberConverter.ConvertDoubleStringToQ16_16("-00387.34080544522"));

	    //Console.WriteLine(Hex(FixedPoint("-00387.34080544522")));

	    Console.WriteLine(FixedPointNumberConverter.ConvertDoubleStringToQ16_16("-00387.34079018644"));
	    Console.WriteLine(FixedPoint("FE7CA8C2"));
	    Console.WriteLine(Hex(FixedPoint("FE7CA8C2")));

	    Console.WriteLine(FixedPointNumberConverter.ConvertQ16_16ToDoubleString("FE7CA8C1"));
	    Console.WriteLine(FixedPoint("FE7CA8C1"));
	    Console.WriteLine(Hex(FixedPoint("FE7CA8C1")));
	    Console.WriteLine(FixedPoint(Hex(FixedPoint("FE7CA8C1"))));
    }

    private static void DotDotDot()
    {
	    string[] vectorA = [
		    FixedPointStr("FF6F42A1"),
		    FixedPointStr("FF37642A"),
		    FixedPointStr("01A1EB33"),
		    FixedPointStr("00EFBEEE")
	    ];
        
	    string[] vectorB = [
		    FixedPointStr("FF5B1E36"),
		    FixedPointStr("0105A213"),
		    FixedPointStr("FFC5F805"),
		    FixedPointStr("017784A2")
	    ];
        
	    string[] aTimesB = [
		    FixedPointNumberConverter.MultiplyFixed(vectorA[0], vectorB[0]),
		    FixedPointNumberConverter.MultiplyFixed(vectorA[1], vectorB[1]),
		    FixedPointNumberConverter.MultiplyFixed(vectorA[2], vectorB[2]),
		    FixedPointNumberConverter.MultiplyFixed(vectorA[3], vectorB[3])
	    ];
        
	    aTimesB.ToList().ForEach(x=> Console.WriteLine(FixedPointNumberConverter.ConvertDoubleStringToQ16_16(x)));
	    Console.WriteLine("Sum of all elements:");
        
	    string sum1 = FixedPointNumberConverter.AddFixed(aTimesB[0], aTimesB[1]);
	    string sum2 =  FixedPointNumberConverter.AddFixed(aTimesB[2], aTimesB[3]);
	    
	    string result =  FixedPointNumberConverter.AddFixed(sum1, sum2);
	     Console.WriteLine(result);
	     Console.WriteLine(FixedPointNumberConverter.ConvertDoubleStringToQ16_16(result));
    }
}