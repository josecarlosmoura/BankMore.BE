namespace CheckingAccountMS.Infrastructure.Utils
{
    public  class CpfValidator
    {
        public static bool EhCpfValido(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11)
                return false;

            // Elimina CPFs inválidos conhecidos (repetidos)
            if (cpf.Distinct().Count() == 1)
                return false;

            var cpfNumeros = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            // Valida primeiro dígito verificador
            var soma1 = 0;
            for (int i = 0; i < 9; i++)
                soma1 += cpfNumeros[i] * (10 - i);

            var resto1 = soma1 % 11;
            var digito1 = resto1 < 2 ? 0 : 11 - resto1;

            if (cpfNumeros[9] != digito1)
                return false;

            // Valida segundo dígito verificador
            var soma2 = 0;
            for (int i = 0; i < 10; i++)
                soma2 += cpfNumeros[i] * (11 - i);

            var resto2 = soma2 % 11;
            var digito2 = resto2 < 2 ? 0 : 11 - resto2;

            return cpfNumeros[10] == digito2;
        }
    }
}
