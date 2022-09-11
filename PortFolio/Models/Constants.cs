
namespace PortFolio.Models;
public static class Constants
{

    public static int AnnimationDuration { get => 5000; }


    public static Byte[] Encoding(this string message)
    {
        // Create a UTF-8 encoding.
        UTF8Encoding utf8 = new UTF8Encoding();

        // A Unicode string with two characters outside an 8-bit code range.
        Console.WriteLine("Original string:");
        Console.WriteLine(message);

        // Encode the string.
        Byte[] encodedBytes = utf8.GetBytes(message);

        // Decode bytes back to string.
       
        return encodedBytes;
    }

}
