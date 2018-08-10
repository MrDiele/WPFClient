using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WpfClient.Model;

namespace WpfClient.WorkWithServer
{
    class InteractionServer
    {
        /// <summary>
        /// Формирует HttpWebRequest.
        /// </summary>
        /// <param name="method">Тип запроса.</param>
        private HttpWebRequest ConWebRequest(string method, string postfixRequest, string param)
        {
            try
            {
                string _postfixRequest = (postfixRequest == null) ? null : "/" + postfixRequest;
                string _param = (param == null) ? null : "/" + param;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:6580/api/Persons" + _postfixRequest + _param);
                webRequest.ContentType = "application/json";
                webRequest.Method = method;
                return webRequest;
            }
            catch (Exception ex)
            {
                //TODO Логи
                return null;
            }
        }

        /// <summary>
        /// Получает от сервера список пользователей.
        /// </summary>
        public List<Person> GetPersonList()
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("GET", null, null);
                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStreamResponse = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStreamResponse);
                    string responseFromServer = reader.ReadToEnd();
                    var resp = JsonConvert.DeserializeObject<List<Person>>(responseFromServer);
                    reader.Close();
                    webResponse.Close();
                    return resp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //TODO Логи
                return null;
            }
        }

        /// <summary>
        /// Получает от сервера список городов.
        /// </summary>
        public List<City> GetCitiesList()
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("GET", "Cities", null);
                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStreamResponse = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStreamResponse);
                    string responseFromServer = reader.ReadToEnd();
                    var resp = JsonConvert.DeserializeObject<List<City>>(responseFromServer);
                    reader.Close();
                    webResponse.Close();
                    return resp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //TODO Логи
                return null;
            }
        }

        /// <summary>
        /// Запрос на добавление нового пользователя.
        /// </summary>
        public void AddNewPerson(Person person, Action action)
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("POST", null, null);
                JObject joe = new JObject(new JProperty("idperson", person.Idperson),
                          new JProperty("name", person.Name),
                          new JProperty("dateofbirth", person.Dateofbirth),
                          new JProperty("city", person.City));
                string s = JsonConvert.SerializeObject(joe);
                byte[] byteArray = Encoding.UTF8.GetBytes(s);
                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    webResponse.Close();
                    action?.Invoke();
                }
            }
            catch (Exception ex)
            {
                //TODO Логи
            }
        }

        /// <summary>
        /// Запрос на изменение данных пользователя.
        /// </summary>
        public void EditPerson(Person person)
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("PUT", null,null);
                JObject joe = new JObject(new JProperty("idperson", person.Idperson),
                                          new JProperty("name", person.Name),
                                          new JProperty("dateofbirth", person.Dateofbirth),
                                          new JProperty("city", person.City));    
                string s = JsonConvert.SerializeObject(joe);
                byte[] byteArray = Encoding.UTF8.GetBytes(s);
                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    webResponse.Close();
                }
            }
            catch (Exception ex)
            {
                //TODO Логи
            }
        }

        /// <summary>
        /// Запрос на удаление пользователя из базы.
        /// </summary>
        public void DeletePerson(int id)
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("DELETE", null, id.ToString());
                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    webResponse.Close();
                }
            }
            catch (Exception ex)
            {
                //TODO Логи
            }
        }

        /// <summary>
        /// Получает от сервера список пользователей с учётом фильтра.
        /// </summary>
        public List<Person> FilterPerson(string filterName, DateTime filterDate, string filterTown)
        {
            try
            {
                HttpWebRequest webRequest = ConWebRequest("POSt", "Filter", null);

                List<JProperty> properties = new List<JProperty>();
                if (filterName != null)
                    properties.Add(new JProperty("name", filterName));
                if(filterDate.ToString() != "01.01.0001 0:00:00")
                    properties.Add(new JProperty("dateofbirth", filterDate));
                if(filterTown != null)
                    properties.Add(new JProperty("city", filterTown));
                JObject joe = new JObject(properties);

                string s = JsonConvert.SerializeObject(joe);
                byte[] byteArray = Encoding.UTF8.GetBytes(s);
                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStreamResponse = webResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStreamResponse);
                    string responseFromServer = reader.ReadToEnd();
                    var resp = JsonConvert.DeserializeObject<List<Person>>(responseFromServer);
                    reader.Close();
                    webResponse.Close();
                    return resp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
                //TODO Логи
            }
        }
    }
}

