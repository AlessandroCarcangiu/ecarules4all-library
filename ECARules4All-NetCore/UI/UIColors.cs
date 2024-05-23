using UnityEngine;
using UnityEngine.Rendering;


// to be removed
namespace ECARules4AllPack.UI
{
    public class UIColors
    {
        public static Color ToRGBA(uint hex)
        {
            float r = ((hex >> 24) & 0xFF) / 255f;
            float g = ((hex >> 16) & 0xFF) / 255f;
            float b = ((hex >> 8) & 0xFF) / 255f;
            float a = (hex & 0xFF) / 255f;
            return new Color(r, g, b, a);
        }
        
        //d3.schemeCategory10
        public static readonly Color blue = ToRGBA(0xff1f77b4);
        public static readonly Color orange = ToRGBA(0xffff7f0e);
        public static readonly Color green = ToRGBA(0xff2ca02c);
        public static readonly Color red = ToRGBA(0xffd62728);
        public static readonly Color purple = ToRGBA(0xff9467bd);
        public static readonly Color brown = ToRGBA(0xff8c564b);
        public static readonly Color pink = ToRGBA(0xffe377c2);
        public static readonly Color gray = ToRGBA(0xff7f7f7f);
        public static readonly Color grey = Color.gray;
        public static readonly Color yellow = ToRGBA(0xffbcbd22);
        public static readonly Color cyan = ToRGBA(0xff17becf);
        public static readonly Color white = ToRGBA(0xffffffff);
        public static readonly Color dropDownColor = ToRGBA(0xff2BC3F4);
    }
}