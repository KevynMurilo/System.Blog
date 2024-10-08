namespace System.Blog.Infrastructure.Services;

public static class EmailTemplates
{
    public static string GenerateVerificationEmailBody(string verificationCode, string name)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                    color: #333;
                }}
                .container {{
                    width: 100%;
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #ffffff;
                    padding: 20px;
                    box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    background-color: #007bff;
                    color: #ffffff;
                    padding: 10px 0;
                    text-align: center;
                    font-size: 24px;
                    font-weight: bold;
                }}
                .content {{
                    padding: 20px;
                    font-size: 16px;
                    line-height: 1.5;
                    color: #333333;
                }}
                .verification-code {{
                    background-color: #f4f4f4;
                    border: 1px solid #ddd;
                    font-size: 18px;
                    padding: 10px;
                    text-align: center;
                    margin: 20px 0;
                    display: block;
                }}
                .footer {{
                    margin-top: 20px;
                    text-align: center;
                    font-size: 12px;
                    color: #888;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    Confirme seu endereço de e-mail
                </div>
                <div class='content'>
                    <p>Olá {name},</p>
                    <p>Obrigado por se registrar no nosso sistema. Para confirmar seu endereço de e-mail, utilize o código de verificação abaixo:</p>
                    <span class='verification-code'>{verificationCode}</span>
                    <p>Se você não solicitou essa verificação, por favor, ignore este e-mail.</p>
                </div>
                <div class='footer'>
                    &copy; {DateTime.Now.Year} Kevyn.Dev. Todos os direitos reservados.
                </div>
            </div>
        </body>
        </html>";
    }
}
