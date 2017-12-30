
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Engine
{
    [Serializable]
    public class Data
    {
        public static event Action Loaded;
        public static event Action Saved;
        private static Dictionary<string, Data> datas = new Dictionary<string, Data>();

        public string id;

        /// <summary>
        /// Saving serializable object to data
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="data">Object to be serialized must be Serializable</param>
        /// <returns></returns>
        public static bool Save(string fileName, object data)
        {
            bool saved = false;
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Create(Application.persistentDataPath + string.Format("/{0}", fileName));
                bf.Serialize(file, data);
            }
            catch (Exception ex)
            {
                saved = false;
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    saved = true;
                }
                else
                {
                    saved = false;
                }
            }
            if (Saved != null)
            {
                Saved();
            }
            if (saved)
            {
                Debug.Log("Save successful");
            }
            else
            {
                Debug.LogError("Didn't save");
            }
            return saved;
        }

        public static bool SaveByte(string fileName, object data)
        {
            bool saved = false;
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Create(Application.persistentDataPath + string.Format("/{0}", fileName));
                bf.Serialize(file, data);
            }
            catch (Exception ex)
            {
                saved = false;
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    saved = true;
                }
                else
                {
                    saved = false;
                }
            }
            if (Saved != null)
            {
                Saved();
            }
            return saved;
        }

        /// <summary>
        /// Loading object from file, remember that it has to be the same type that was saved to a file
        /// </summary>
        /// <typeparam name="T">Object's class</typeparam>
        /// <param name="fileName">file to load from</param>
        /// <returns>Returns an object loaded from file</returns>
        public static object Load(string fileName)
        {
            object data = null;
            FileStream file = null;
            try
            {
                string path = Application.persistentDataPath + string.Format("/{0}", fileName);
                if (File.Exists(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(path, FileMode.Open);
                    data = bf.Deserialize(file);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            if (Loaded != null)
            {
                Loaded();
            }
            return data;
        }

    //    public static void Serialize<T>(T data, Stream stream)
    //    {
    //        try // try to serialize the collection to a file
    //        {
    //            using (stream)
    //            {
    //                // create BinaryFormatter
    //                BinaryFormatter bin = new BinaryFormatter();
    //                // serialize the collection (EmployeeList1) to file (stream)
    //                bin.Serialize(stream, data);
    //            }
    //        }
    //        catch (IOException)
    //        {
    //        }
    //    }

    //    public static T Deserialize<T>(Stream stream) where T : new()
    //    {
    //        T ret = CreateInstance<T>();
    //        try
    //        {
    //            using (stream)
    //            {
    //                // create BinaryFormatter
    //                BinaryFormatter bin = new BinaryFormatter();
    //                // deserialize the collection (Employee) from file (stream)
    //                ret = (T)bin.Deserialize(stream);
    //            }
    //        }
    //        catch (IOException)
    //        {
    //        }
    //        return ret;
    //    }
    //    // function to create instance of T
    //    public static Object CreateInstance<Object>() where Object : new()
    //    {
    //        return (Object)Activator.CreateInstance(typeof(Object));
    //    }
    }

    [Serializable]
    public class STransform
    {
        public Vector LocalPosition { get; set; }
        public Vector LocalScale { get; set; }
        public Vector LocalRotation { get; set; }

        public static implicit operator STransform(Transform trans)
        {
            return new STransform()
            {
                LocalPosition = trans.localPosition,
                LocalScale = trans.localScale,
                LocalRotation = trans.localRotation
            };
        }
    }

    public interface IData
    {
        string ID { get; }
    }



}