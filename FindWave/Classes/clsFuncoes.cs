using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace FindWave.Classes
{
    class clsFuncoes
    {
        public static bool IsNumeric(KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string RetornaNumero(string _texto)
        {
            Regex r = new Regex(@"\d+");

            string result = "";

            foreach (Match m in r.Matches(_texto))
            {
                result += m.Value;
            }

            if (result != "")
            {
                Double i = 0;
                i = Convert.ToDouble(result);

                result = i.ToString();
            }
            else
            {
                result = "0";
            }

            return result;
        }

        public static Boolean ValidaFone(string _fone)
        {
            Boolean booV = false;

            switch (_fone.Length)
            {
                case 10:
                    string x = _fone.Substring(2, 1);

                    switch (_fone.Substring(2, 1))
                    {
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                            booV = true;
                            break;
                    }
                    break;

                case 11:
                    if (_fone.Substring(2, 1) == "9")
                    {
                        booV = true;
                    }
                    break;

                default:
                    break;
            }

            return booV;
        }


    }
}
