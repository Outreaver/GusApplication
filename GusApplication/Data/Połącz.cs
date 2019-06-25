using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using GusApplication.GusAPI;

namespace GusApplication.Data
{
    class Połącz
    {
        private UslugaBIRzewnPublClient klient;

        public UslugaBIRzewnPublClient Klient
        {
            get { return this.klient; }
        }

        public Połącz(UslugaBIRzewnPublClient klient)
        {
            this.klient = klient;
        }
        public string Zaloguj()
        {
            string sid = this.klient.Zaloguj("abcde12345abcde12345");
            OperationContextScope scope = new OperationContextScope(klient.InnerChannel);

            HttpRequestMessageProperty reqProps = new HttpRequestMessageProperty();
            reqProps.Headers.Add("sid", sid);
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = reqProps;

            return sid;
        }

        public void Wylogoj(string adres)
        {
            klient.Wyloguj(adres);
        }
    }
}
