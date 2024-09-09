using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


public class ModValidacao
{
    private string Cartao;
    private string CPF;
    private string Senha;
    private string Conta;
    private string identificador = "KmDnbOpoasnJG";
    private string chaveSecreta = "Chave";
    private int minutosExpiracao = 3; 
    public ModValidacao(string _cartao, string _CPF, string _senha, string _conta)
    {
        this.CPF = _CPF;
        this.Cartao = _cartao;
        this.Conta = _conta;
        this.Senha = _senha;

    }

    public string Master(){
    try
    {
        if (ValidarCPF(CPF))
        {
            if (ValidarSenha(Senha))
            {
                if (ValidarCartao(Cartao))
                {
                    if (ValidarConta(Conta))
                    {
                        string Token = Gerar_Token(identificador, chaveSecreta, minutosExpiracao);
                        
                        var json = new 
                        {
                            token = Token,
                            cpf = CPF,
                            cartao = Cartao,
                            conta = Conta,
                            senha = Senha
                        };

                        string jsonString = JsonSerializer.Serialize(json);
                        return jsonString;
                    }
                }
            }
        }
        return "Dados inválidos";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
        return $"Erro ao processar dados: {ex.Message}";
    }

    }
    public bool ValidarCPF(string CPF)
    {
        //remove possiveis espaços no inicio e fim da string _CPF
        string _CPF = CPF.Trim();
        
        //aplica o pattern de um regex para cpf
        string cpfPattern = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$";
        Regex regex = new Regex(cpfPattern);

        if (string.IsNullOrWhiteSpace(_CPF))
        {
              throw new ArgumentException("O CPF não pode ser nulo ou conter apenas espaços em branco.", nameof(_CPF));
        }

        return regex.IsMatch(_CPF);
    }

    public bool ValidarCartao(string Cartao)
    {
        string _cartao = Cartao.Trim();
        string cardPattern = @"^(\d{4}[- ]?){3}\d{4}|\d{15,19}$";
        Regex regex = new Regex(cardPattern);
        _cartao = _cartao.Replace(" ", "").Replace("-", "");

        if (string.IsNullOrWhiteSpace(_cartao))
        {
            throw new ArgumentException("O Cartão não pode ser nulo ou conter espaços apenas espaços em branco", nameof(_cartao));
        }

        if (!regex.IsMatch(_cartao))
        {
            throw new ArgumentException("O Cartão não esta no formato adequado", nameof(_cartao));
        }

        try
        {
            ValidarNuhn(Cartao);
            return true;
        } 
        catch(ArgumentException e){
            Console.WriteLine(e);
            return false;
        }
        
        
    }

    private bool ValidarNuhn(string _cartao){
        int soma = 0;
        bool dobrar = false;

        // Itera sobre os dígitos do cartão de trás para frente
        for (int i = _cartao.Length - 1; i >= 0; i--)
        {
            int digito = _cartao[i] - '0'; // Converte o caractere para um dígito

            if (dobrar)
            {
                digito *= 2;
                if (digito > 9)
                {
                    digito -= 9;
                }
            }

            soma += digito;
            dobrar = !dobrar; // Alterna entre dobrar e não dobrar
        }


        if (soma % 10 == 0) 
        {
            return true;
        }
        else 
        {
            throw new ArgumentException("Cartão invalido", nameof(_cartao));
        }
    }

    public bool ValidarConta(string _conta)
    {
        _conta = _conta.Trim();
        string AccountPattern = @"^\d{5}-\d{1}$";
        Regex regex = new Regex(AccountPattern);
        
        return regex.IsMatch(_conta);
    }

    public string Gerar_Token(string _indentificador, string _chaveSecreta, int minutosExpiracao)
    {
        // Obtém o timestamp atual e o timestamp de expiração
        DateTime dataExpiracao = DateTime.UtcNow.AddMinutes(minutosExpiracao);
        string timestamp = dataExpiracao.ToString("o");

        // Concatena identificador com timestamp
        string msgComTimeStamp = $"{_indentificador}|{timestamp}";

        // Converte a chave secreta e a mensagem para bytes
        byte[] chaveBytes = Encoding.UTF8.GetBytes(_chaveSecreta);
        byte[] mensagemBytes = Encoding.UTF8.GetBytes(msgComTimeStamp);

         // Cria uma instância do HMAC
        using (HMACSHA256 hmac = new HMACSHA256(chaveBytes))
        {
            // Gera o HMAC da mensagem
            byte[] hashBytes = hmac.ComputeHash(mensagemBytes);

            // Converte o hash para uma string hexadecimal
            string token = BitConverter.ToString(hashBytes).Replace("-","").ToLower();
            
            return $"{timestamp}|{token}";
        }

    }

    public bool ValidarSenha(string _senha){
        if (_senha.Length >= 7) {
            throw new ArgumentException("SENHA COM TAMANHO INVALIDO", nameof(_senha));
        }

        return true;

    }
    
}