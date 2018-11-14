using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebGateCr.Models.Data;

namespace WebGateCr.Models.AxaptaModel.ATS
{

    /// <summary>
    ///  112 Http- request incoming params
    /// </summary>
    public class NVAATS_PARAMS
    {
        public string uid       { get; set; }
        public string pwd       { get; set; }
        public string aNumber   { get; set; }
        public string method     { get; set; }
    }


    public class Resp112Helper
    {
        private Dictionary<int, string> Errors = new Dictionary<int, string>();

        // internal switcher helper
        private class Resp<T>{ public int Err { get; set; }  public T Body { get; set; }
            public Resp(int err, T body) { this.Err = err; this.Body = body; }
            public Resp<Y> Step<Y>( Func<T, bool> condition, int newError, Func< T, Y > fun, Y errVal) 
            {
                return this.Err != 0 ? 
                    new Resp<Y>(this.Err , errVal ) :
                    condition(this.Body) ? new Resp<Y>(this.Err,  fun(this.Body)) : new Resp<Y>(newError , errVal ) ;
            }
            public Resp<T> Step( Func<T, bool> condition, int newError, Func<T, T> fun ) => this.Step<T>(condition, newError, fun, this.Body);
        };


        // internal switcher helper v2
        //private class Tubule<T>
        //{
            
        //    public int Err { get; set; }
        //    public T Body { get; set; }
        //    public Tubule(int err, T body) { Err = err; Body = body; }
        //    public Resp<Y> StepMap<Y>( (Func<T, bool> iff, int thenErr, Y valErr, Func<T, Y> elseMap ) inp )
        //    {
        //        return this.Err != 0 ?
        //            new Resp<Y>(this.Err, errVal) :
        //            condition(this.Body) ? new Resp<Y>(this.Err, fun(this.Body)) : new Resp<Y>(newError, errVal);
        //    }
        //    public Resp<T> Step((Func<T, bool> iff, int thennErr) inp ) => this.StepMap<T>((iff: inp.iff, inp.thennErr, (x => x) , this.Body));
        //};


        public Resp112Helper()
        {
            Errors.Add(0, "OK");
            Errors.Add(1, "SYSTEM FAILURE");
            Errors.Add(2, "UNSPECIFIED ERROR");
            Errors.Add(3, "UNAUTHORIZED APPLICATION");     ///!!!!
            Errors.Add(4, "UNKNOWN SUBSCRIBER");
            Errors.Add(5, "ABSENT SUBSCRIBER");
            Errors.Add(6, "POSITION METHOD FAILURE");
            Errors.Add(105, "FORMAT ERROR");
            Errors.Add(106, "SYNTAX EROR");
        }

        public string BuildResponse(NVAATS_PARAMS _pars, IAxCommon context) {

            //var 1 = (1) 

            var proc = new Resp<(NVAATS_PARAMS prs, IAxCommon ctx)>(0, (_pars, context))
                .Step<(NVAATS_PARAMS prs, IAxCommon ctx, NVAATS_SETTING stng)>(
                    x => x.ctx.NvaAtsSetting.Access.Reader.GetAll().Count() > 0,
                    1,                                                                                          // Не вижу сеттинга                                                                            
                    x => (x.prs, x.ctx, x.ctx.NvaAtsSetting.Access.Reader.GetAll().First(r => true)),
                    (null, null, null)
                )
                .Step(
                    x =>
                        x.stng.ISAUTHONLY == 0
                        || (x.prs.uid != null
                            && ((
                                x.prs.pwd != null
                                && x.prs.pwd == x.stng.PASS
                                && x.prs.uid == x.stng.LOGIN
                            ) || (
                                x.prs.pwd != null
                                && x.prs.pwd == x.stng.PASS2
                                && x.prs.uid == x.stng.LOGIN2
                            )
                        )),
                    3,                                                                                      // Auth false   
                    x => x
                )
                .Step(
                    x => x.prs.aNumber != null,
                    105,
                    x => x
                )
                .Step<(NVAATS_PARAMS prs, IAxCommon ctx, NVAATS_SETTING stng, NVAATS_PHONES phn)>(
                    x => x.ctx.NvaAtsPhones.Access.Reader.Find(x.prs.aNumber) != null,
                    4,                                                                                      // Phone not found
                    x => (x.prs, x.ctx, x.stng, x.ctx.NvaAtsPhones.Access.Reader.Find(x.prs.aNumber)),
                    (null, null, null, null)
                )
                .Step<string>(
                    x => true,
                    0,
                    x => this.buildBody(x.prs, x.ctx, x.stng, x.phn),
                    (null)
                )
                .Step(
                    x => x != null,
                    2,
                    x => x 
                );                
                

            return this.BuildWrap(proc.Err, proc.Body) ;
        }


        private string BuildWrap(int _error, string body){

            var sb = new StringBuilder();

            sb.AppendLine("<response>");
            sb.AppendLine("<resultCode>"+_error+ "</resultCode>" );
            sb.AppendLine("<resultDescription>" + ( Errors.ContainsKey(_error) ? Errors[_error]: "ERROR DESCRIPTON IS EMPTY") + "</resultDescription>");
            sb.AppendLine("<AbonentInfo>");
            sb.AppendLine(body);
            sb.AppendLine("</AbonentInfo>");
            sb.AppendLine("</response>");

            return sb.ToString();
        }

        private string buildBody(NVAATS_PARAMS prs, IAxCommon ctx, NVAATS_SETTING stng, NVAATS_PHONES phn)
        {
            try
            {
                var dev = ctx.NvaAtsDevice.Access.Reader.Find(phn.DEVICEID);
                var devid = (dev == null) ? Guid.Empty.ToString() : dev.GUID.ToString();

                var bld = ctx.NvaAtsBuilding.Access.Reader.Find(phn.BUILDINGID);
                var bldid = (bld == null) ? Guid.Empty.ToString() : bld.GUID.ToString();


                var doc = new XmlDocument();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = doc.CreateElement(string.Empty, "presence", "urn:ietf:params:xml:ns:pidf");

                //slide2XmlWriter.WriteAttributeString("xmlns", "a", null, "http://schemas.openxmlformats.org/drawingml/2006/main");


                var ns_dm = "urn:ietf:params:xml:ns:pidf:data-model";
                var ns_gp = "urn:ietf:params:xml:ns:pidf:geopriv10";
                var ns_cl = "urn:ietf:params:xml:ns:pidf:geopriv10:civicLoc";


                element1.SetAttribute("xmlns:dm", ns_dm);
                element1.SetAttribute("xmlns:gp", ns_gp);
                element1.SetAttribute("xmlns:cl", ns_cl);
                element1.SetAttribute("entity", phn.PHONE);
                doc.AppendChild(element1);

                XmlElement element2 = doc.CreateElement("dm", "device", ns_dm);
                element2.SetAttribute("id", devid);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement("gp", "geopriv", ns_gp);
                element2.AppendChild(element3);

                XmlElement element4 = doc.CreateElement("gp", "location-info", ns_gp);
                element3.AppendChild(element4);

                XmlElement element5 = doc.CreateElement("cl", "civicAddress", ns_cl);
                element4.AppendChild(element5);

                var el = doc.CreateElement("cl", "HOUSEGUID", ns_cl);
                el.InnerText = bldid;
                element5.AppendChild(el);

                el = doc.CreateElement("cl", "ENTRANCE", ns_cl);
                el.InnerText = phn.ENTRANCE;
                element5.AppendChild(el);

                el = doc.CreateElement("cl", "LEVEL", ns_cl);
                el.InnerText = phn.LEVEL_;
                element5.AppendChild(el);

                 el = doc.CreateElement("cl", "ROOM", ns_cl);
                el.InnerText = phn.ROOM;
                element5.AppendChild(el);


                element3 = doc.CreateElement("dm", "deviceID", ns_dm);
                element3.InnerText = phn.PHONE;
                element2.AppendChild(element3);

                element3 = doc.CreateElement("dm", "timestamp", ns_dm);
                element3.InnerText = DateTime.Now.ToString(); //DateTime.Now.fo.ToLongDateString()+ "T"+DateTime.Now.ToLongTimeString()+"Z";
                element2.AppendChild(element3);


                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
            catch (Exception e )
            {
                return null;
            }

        }    

    }
}
