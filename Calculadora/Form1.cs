using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculadora
{
    public partial class Calculadora : Form
    {
        public Calculadora()
        {
            InitializeComponent();
        }

        private Double numero1;
        private Double numero2;
        private String operação;

        private void Calculadora_Load(object sender, EventArgs e)
        {
            LimparCampos();
        }
        private void LimparCampos()
        {
            txtDisplay.Clear();
            numero1 = 0;
            numero2 = 0;
            operação = String.Empty;

        }
        private void AdicionarNúmero(String caracter)
        {
            txtDisplay.Text = txtDisplay.Text + caracter;

        }

        private void AdicionarOperação(String caracter)
        {
            if (!txtDisplay.Text.Trim().Equals(String.Empty))
            {
                numero1 = Convert.ToDouble(txtDisplay.Text.Trim());
                operação = caracter;
                txtDisplay.Clear();
            }

        }

        private void Calcular()
        {

            if (operação == "+")
            {
                txtDisplay.Text = (numero1 + numero2).ToString();
            }
            else if (operação == "÷")
            {
                txtDisplay.Text = (numero1 / numero2).ToString();
            }
            else if (operação == "x")
            {
                txtDisplay.Text = (numero1 * numero2).ToString();
            }
            else if (operação == "-")
            {
                txtDisplay.Text = (numero1 - numero2).ToString();
            }

            /*
            switch (operação)
            {
                case "÷":
                    txtDisplay.Text = (numero1 / numero2).ToString();
                    break;
                case "x":
                    txtDisplay.Text = (numero1 * numero2).ToString();
                    break;
                
            }

            */

        }
        private void btn0_Click(object sender, EventArgs e)
        {
            if (!txtDisplay.Text.Trim().Equals("0"))

                AdicionarNúmero("0");
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("1");
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("2");
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("3");
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("4");
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("5");
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("6");
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("7");
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("8");
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            AdicionarNúmero("9");
        }

        private void btnDivisão_Click(object sender, EventArgs e)
        {
            AdicionarOperação("÷");
        }

        private void btnMultiplicação_Click(object sender, EventArgs e)
        {
            AdicionarOperação("x");
        }

        private void btnSubtração_Click(object sender, EventArgs e)
        {
            AdicionarOperação("-");
        }

        private void btnAdição_Click(object sender, EventArgs e)
        {
            AdicionarOperação("+");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtDisplay.Text!=string.Empty)txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            if (!txtDisplay.Text.Trim().Equals(String.Empty))
            {
                numero2 = Convert.ToDouble(txtDisplay.Text.Trim());
                Calcular();
            }
        }
    }
}
