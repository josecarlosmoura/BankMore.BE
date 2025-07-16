namespace CheckingAccountMS.Infrastructure.Utils
{
    public class ContaCorrenteGenerator
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Gera um número de conta corrente com dígito verificador no padrão brasileiro (ex: 123456-7).
        /// </summary>
        public static string GerarNumeroContaComDV()
        {
            var numeroBase = _random.Next(100000, 999999).ToString(); // 6 dígitos
            var dv = CalcularDigitoVerificador(numeroBase);

            return $"{numeroBase}-{dv}";
        }

        /// <summary>
        /// Calcula o dígito verificador com base no algoritmo do módulo 11 com pesos decrescentes.
        /// </summary>
        private static int CalcularDigitoVerificador(string numeroBase)
        {
            int soma = 0;
            int peso = 2;

            for (int i = numeroBase.Length - 1; i >= 0; i--)
            {
                soma += (numeroBase[i] - '0') * peso;
                peso = peso == 9 ? 2 : peso + 1;
            }

            int resto = soma % 11;

            if (resto == 0 || resto == 1) return 0;
            return 11 - resto;
        }
    }
}
