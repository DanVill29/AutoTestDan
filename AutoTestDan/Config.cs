using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTestDan
{
    [Serializable]
    public class Config : ICloneable
    {
        public string environment = "QA";
        public string browser = "chrome";
        public string url = @"https://www.gmail.com";


        public override string ToString()
        {
            return String.Format("environment: {0} browser: {1} url: {2}", this.environment, this.browser, this.url);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
