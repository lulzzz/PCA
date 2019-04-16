using System.Collections.Generic;

namespace PrivateCert.LibCore.Model
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
