namespace AraneaOculus.Core.Models.Identification
{
    public class Identifier
    {
        public Identifier(Guid token, double probability)
        {
            Token = token;
            Probability = probability;
        }

        public readonly Guid Token;

        public readonly double Probability;
    }
}
