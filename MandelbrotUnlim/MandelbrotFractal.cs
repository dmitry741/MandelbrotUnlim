using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongFloatLib;

namespace MandelbrotUnlim
{
    /// <summary>
    ///  Фрактал Мандельброта.
    /// </summary>
    public class MandelbrotFractal : AbstractDynamicFractal
    {
        readonly LongFloat cTolerance = LongFloat.FromDouble(4);

        protected override Complex NextIteration(Complex z)
        {
            return z * z + Start;
        }

        protected override bool Check(Complex z)
        {
            return z.ModuleInSquare > cTolerance;
        }

        public override AbstractDynamicFractal Copy() { return new MandelbrotFractal(); }
    }
}
