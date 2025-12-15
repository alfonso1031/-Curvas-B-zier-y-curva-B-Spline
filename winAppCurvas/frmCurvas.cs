using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace winAppCurvas
{
    public partial class frmCurvas : Form
    {
        // Lista para almacenar los puntos de control
        private List<PointF> puntosControl;

        // Instancias de los algoritmos
        private CBezier bezierAlgo;
        private CBSpline bsplineAlgo;

        public frmCurvas()
        {
            InitializeComponent();
            puntosControl = new List<PointF>();
            bezierAlgo = new CBezier();
            bsplineAlgo = new CBSpline();

            // Selección por defecto
            cmbAlgoritmo.SelectedIndex = 0;

            // Configurar PictureBox para doble buffer (evitar parpadeo)
            picLienzo.GetType().GetMethod("SetStyle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Invoke(picLienzo, new object[] { ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true });
        }

        private void picLienzo_MouseClick(object sender, MouseEventArgs e)
        {
            // Validar entrada: Coordenadas dentro del rango visible y positivas
            if (e.X < 0 || e.Y < 0 || e.X > picLienzo.Width || e.Y > picLienzo.Height)
            {
                MessageBox.Show("Coordenadas fuera de rango.");
                return;
            }

            // Agregar punto
            puntosControl.Add(new PointF(e.X, e.Y));

            // Actualizar vista
            ActualizarEstado();
            picLienzo.Invalidate();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            puntosControl.Clear();
            lblInfo.Text = "Puntos: 0";
            picLienzo.Invalidate();
        }

        private void picLienzo_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Dibujar puntos de control y líneas guía (Hull)
            if (puntosControl.Count > 0)
            {
                // Dibujar puntos
                foreach (var p in puntosControl)
                {
                    e.Graphics.FillEllipse(Brushes.Red, p.X - 3, p.Y - 3, 6, 6);
                }

                // Dibujar líneas guía (polígono de control) gris claro
                if (puntosControl.Count > 1)
                {
                    e.Graphics.DrawLines(Pens.LightGray, puntosControl.ToArray());
                }
            }

            // 2. Dibujar la curva seleccionada
            try
            {
                if (cmbAlgoritmo.SelectedItem.ToString() == "Bézier")
                {
                    // Bézier necesita exactamente 4 puntos para este ejemplo o lógica segmentada
                    if (puntosControl.Count == 4)
                    {
                        bezierAlgo.Dibujar(e.Graphics, Pens.Blue, puntosControl);
                    }
                    else if (puntosControl.Count > 0 && puntosControl.Count < 4)
                    {
                        // Feedback visual opcional
                    }
                }
                else if (cmbAlgoritmo.SelectedItem.ToString() == "B-Spline")
                {
                    // B-Spline funciona mejor con 4 o más puntos
                    if (puntosControl.Count >= 4)
                    {
                        bsplineAlgo.Dibujar(e.Graphics, Pens.Green, puntosControl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al dibujar: " + ex.Message);
            }
        }

        private void cmbAlgoritmo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarEstado();
            picLienzo.Invalidate();
        }

        private void ActualizarEstado()
        {
            string algoritmo = cmbAlgoritmo.SelectedItem?.ToString();
            int cantidad = puntosControl.Count;

            string msj = $"Algoritmo: {algoritmo} | Puntos: {cantidad}";

            if (algoritmo == "Bézier")
            {
                if (cantidad < 4) msj += " (Requiere 4 puntos)";
                else if (cantidad > 4) msj += " (Usando primeros 4)";
            }
            else // B-Spline
            {
                if (cantidad < 4) msj += " (Requiere mín. 4 puntos)";
            }

            lblInfo.Text = msj;
        }
    }
}