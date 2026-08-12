using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBR
{


    /// <summary>
    /// Object is storing information about credits
    /// List of this class is created and populated in UcLanguage.cs in FillCreditList()
    /// </summary>
    public class CreditsInfo
    {
        public string LanguageCode { get; set; } // like en-gb

        public string CountryLanguage { get; set; } // like United Kingdom (Ennglish)

        public string Name { get; set; } // like John Smith - translator

        public string HyperLinkUrl { get; set; } // like https://www.example.com

        public string HyperLinkText { get; set; } // like "J.S. Agency" or URL can be used here as well

    }
}
