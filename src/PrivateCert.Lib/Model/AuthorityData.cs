using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Lib.Model
{
    public class AuthorityData
    {
        public int CertificateId { get; set; }

        public string FirstP7B { get; set; }

        public string SecondP7B { get; set; }

        public ICollection<string> P7Bs
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(FirstP7B))
                {
                    list.Add(FirstP7B);
                }

                if (!string.IsNullOrEmpty(SecondP7B))
                {
                    list.Add(SecondP7B);
                }

                return list;
            }
        }
    }
}
