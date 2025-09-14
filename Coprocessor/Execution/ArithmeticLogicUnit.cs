using Coprocessor.Structures;

namespace Coprocessor.Execution;

public static class ArithmeticLogicUnit
{
    
    // ---Arithmetic---

    public static Vector Add(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(a[i] + b[i]);

        return result;
    }

    public static Vector Sub(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(a[i] - b[i]);

        return result;
    }


    // ---Bitwise---

    public static Vector And(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(a[i] & b[i]);

        return result;
    }

    public static Vector Or(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(a[i] | b[i]);

        return result;
    }

    public static Vector Xor(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(a[i] ^ b[i]);

        return result;
    }

    public static Vector Not(Vector a)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(~a[i]);

        return result; 
    }

    public static Vector Implies(Vector a, Vector b)
    {
        Vector result = new Vector();

        for (int i = 0; i < Vector.length; i++)
            result[i] = (byte)(~a[i] | b[i]);

        return result;
    }
}
