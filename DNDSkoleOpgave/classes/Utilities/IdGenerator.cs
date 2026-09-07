using System.Security.Cryptography;

namespace DNDSkoleOpgave.Utilities;

public static class IdGenerator
{
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public const int Length = 10;

    public static string Create()
    {
        Span<char> id = stackalloc char[Length];
        for (int index = 0; index < id.Length; index++)
        {
            id[index] = Characters[RandomNumberGenerator.GetInt32(Characters.Length)];
        }

        return new string(id);
    }
}
