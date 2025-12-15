using System;
using System.Collections.Generic;
using System.Drawing;

namespace winAppCurvas
{
    /// <summary>
    /// Clase encargada de la lógica y dibujo de Curvas B-Spline.
    /// Implementa B-Splines Cúbicas Uniformes por aproximación.
    /// </summary>
    public class CBSpline
    {
        /// <summary>
        /// Dibuja la curva B-Spline para un conjunto de N puntos de control.
        /// </summary>
        /// <param name="g">Objeto Graphics.</param>
        /// <param name="pen">Lápiz de dibujo.</param>
        /// <param name="puntos">Lista de puntos de control.</param>
        public void Dibujar(Graphics g, Pen pen, List<PointF> puntos)
        {
            // B-Spline Cúbica requiere al menos 4 puntos para definir el primer segmento
            if (puntos == null || puntos.Count < 4)
                return;

            try
            {
                // Una B-Spline con N puntos tiene N-3 segmentos
                for (int i = 0; i < puntos.Count - 3; i++)
                {
                    DibujarSegmento(g, pen, puntos[i], puntos[i + 1], puntos[i + 2], puntos[i + 3]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al calcular B-Spline: " + ex.Message);
            }
        }

        /// <summary>
        /// Calcula y dibuja un segmento basado en 4 puntos de control locales.
        /// Usa la matriz base de B-Spline.
        /// </summary>
        private void DibujarSegmento(Graphics g, Pen pen, PointF p0, PointF p1, PointF p2, PointF p3)
        {
            PointF pAnterior = CalcularPuntoBSpline(0, p0, p1, p2, p3);

            for (double t = 0; t <= 1; t += 0.05)
            {
                PointF pActual = CalcularPuntoBSpline(t, p0, p1, p2, p3);
                g.DrawLine(pen, pAnterior, pActual);
                pAnterior = pActual;
            }
        }

        /// <summary>
        /// Fórmula matricial expandida para B-Spline Cúbica Uniforme.
        /// P(t) = 1/6 * [t^3 t^2 t 1] * M * [P0 P1 P2 P3]^T
        /// </summary>
        private PointF CalcularPuntoBSpline(double t, PointF p0, PointF p1, PointF p2, PointF p3)
        {
            double t2 = t * t;
            double t3 = t * t * t;

            // Funciones base calculadas manualmente para eficiencia
            // b0 = (1-t)^3 / 6
            // b1 = (3t^3 - 6t^2 + 4) / 6
            // b2 = (-3t^3 + 3t^2 + 3t + 1) / 6
            // b3 = t^3 / 6

            // Aplicando coeficientes directos:
            double c0 = (-t3 + 3 * t2 - 3 * t + 1) / 6.0;
            double c1 = (3 * t3 - 6 * t2 + 4) / 6.0;
            double c2 = (-3 * t3 + 3 * t2 + 3 * t + 1) / 6.0;
            double c3 = t3 / 6.0;

            float x = (float)(c0 * p0.X + c1 * p1.X + c2 * p2.X + c3 * p3.X);
            float y = (float)(c0 * p0.Y + c1 * p1.Y + c2 * p2.Y + c3 * p3.Y);

            return new PointF(x, y);
        }
    }
}