using System;
using System.Collections.Generic;
using System.Drawing;

namespace winAppCurvas
{
    /// <summary>
    /// Clase encargada de la lógica y dibujo de Curvas de Bézier.
    /// Implementa el algoritmo utilizando polinomios de Bernstein.
    /// </summary>
    public class CBezier
    {
        /// <summary>
        /// Dibuja una curva de Bézier cúbica basada en 4 puntos de control.
        /// </summary>
        /// <param name="g">Objeto Graphics donde se dibujará.</param>
        /// <param name="pen">Lápiz para el dibujo.</param>
        /// <param name="puntos">Lista de puntos de control.</param>
        public void Dibujar(Graphics g, Pen pen, List<PointF> puntos)
        {
            // Validación: Se requieren al menos 4 puntos para una Bézier cúbica completa
            if (puntos == null || puntos.Count < 4)
                return;

            try
            {
                // Iteramos en pasos pequeños de 't' (de 0 a 1)
                // 0.01 proporciona una resolución suave
                PointF pAnterior = puntos[0];

                // Nota: Este bucle dibuja un segmento cúbico usando los primeros 4 puntos.
                // Para N puntos en cadena, se requeriría una lógica de empalme, 
                // pero aquí demostramos el algoritmo fundamental cúbico.
                for (double t = 0; t <= 1; t += 0.01)
                {
                    PointF pActual = CalcularPuntoBezier(t, puntos[0], puntos[1], puntos[2], puntos[3]);
                    g.DrawLine(pen, pAnterior, pActual);
                    pAnterior = pActual;
                }

                // Asegurar que llegue al último punto exacto
                g.DrawLine(pen, pAnterior, puntos[3]);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al calcular Bézier: " + ex.Message);
            }
        }

        /// <summary>
        /// Fórmula matemática de Bézier Cúbica: B(t) = (1-t)^3*P0 + 3(1-t)^2*t*P1 + 3(1-t)*t^2*P2 + t^3*P3
        /// </summary>
        private PointF CalcularPuntoBezier(double t, PointF p0, PointF p1, PointF p2, PointF p3)
        {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            double uuu = uu * u;
            double ttt = tt * t;

            float x = (float)(uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X);
            float y = (float)(uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y);

            return new PointF(x, y);
        }
    }
}