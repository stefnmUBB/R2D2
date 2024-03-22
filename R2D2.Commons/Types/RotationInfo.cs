using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2D2.Commons.Types
{
    public class RotationInfo
    {
        public char Axis;
        public double Value;
        public RotationInfo(char axis, double value)
        {
            Axis = axis;
            Value = value;
        }
    }
}
