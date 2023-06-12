using System;
using UnityEngine;

namespace Unity.MLAgents.Sensors
{
    /// <summary>
    /// Sensor class that wraps a [RenderTexture](https://docs.unity3d.com/ScriptReference/RenderTexture.html) instance.
    /// </summary>
    /// 

    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

        /// <summary>
        /// Component that wraps a <see cref="RenderTextureSensor"/>.
        /// </summary>
        
        public class SkinSensorComponent : SensorComponent, IDisposable
        {
            SkinSensor m_Sensor;

            /// <summary>
            /// The [RenderTexture](https://docs.unity3d.com/ScriptReference/RenderTexture.html) instance
            /// that the associated <see cref="RenderTextureSensor"/> wraps.
            /// </summary>
            [HideInInspector, SerializeField, FormerlySerializedAs("renderTexture")]
            Texture2D m_texture2D;

            /// <summary>
            /// Stores the [RenderTexture](https://docs.unity3d.com/ScriptReference/RenderTexture.html)
            /// associated with this sensor.
            /// </summary>
            public Texture2D texture2D
            {
                get { return m_texture2D; }
                set { m_texture2D = value; }
            }

            [HideInInspector, SerializeField, FormerlySerializedAs("sensorName")]
            string m_SensorName = "RenderTextureSensor";

            /// <summary>
            /// Name of the generated <see cref="RenderTextureSensor"/>.
            /// Note that changing this at runtime does not affect how the Agent sorts the sensors.
            /// </summary>
            public string SensorName
            {
                get { return m_SensorName; }
                set { m_SensorName = value; }
            }

            [HideInInspector, SerializeField, FormerlySerializedAs("grayscale")]
            bool m_Grayscale;

            /// <summary>
            /// Whether the RenderTexture observation should be converted to grayscale or not.
            /// Note that changing this after the sensor is created has no effect.
            /// </summary>
            public bool Grayscale
            {
                get { return m_Grayscale; }
                set { m_Grayscale = value; }
            }

            [HideInInspector, SerializeField]
            [Range(1, 50)]
            [Tooltip("Number of frames that will be stacked before being fed to the neural network.")]
            int m_ObservationStacks = 1;

            [HideInInspector, SerializeField, FormerlySerializedAs("compression")]
            SensorCompressionType m_Compression = SensorCompressionType.PNG;

            /// <summary>
            /// Compression type for the render texture observation.
            /// </summary>
            public SensorCompressionType CompressionType
            {
                get { return m_Compression; }
                set { m_Compression = value; UpdateSensor(); }
            }
        public SensingTouch sensingTouch;
            /// <summary>
            /// Whether to stack previous observations. Using 1 means no previous observations.
            /// Note that changing this after the sensor is created has no effect.
            /// </summary>
            public int ObservationStacks
            {
                get { return m_ObservationStacks; }
                set { m_ObservationStacks = value; }
            }

            /// <inheritdoc/>
            public override ISensor[] CreateSensors()
            {
                Dispose();
                m_Sensor = new SkinSensor(sensingTouch, texture2D, Grayscale, SensorName, m_Compression);
                if (ObservationStacks != 1)
                {
                    return new ISensor[] { new StackingSensor(m_Sensor, ObservationStacks) };
                }
                return new ISensor[] { m_Sensor };
            }

            /// <summary>
            /// Update fields that are safe to change on the Sensor at runtime.
            /// </summary>
            internal void UpdateSensor()
            {
                if (m_Sensor != null)
                {
                    m_Sensor.CompressionType = m_Compression;
                }
            }

            /// <summary>
            /// Clean up the sensor created by CreateSensors().
            /// </summary>
            public void Dispose()
            {
                if (!ReferenceEquals(null, m_Sensor))
                {
                    m_Sensor.Dispose();
                    m_Sensor = null;
                }
            }
        }
    

    public class SkinSensor : ISensor, IDisposable
    {
        //RenderTexture m_RenderTexture;
        bool m_Grayscale;
        string m_Name;
        private ObservationSpec m_ObservationSpec;
        SensorCompressionType m_CompressionType;
        Texture2D m_Texture;
        SensingTouch sensingTouch;

        /// <summary>
        /// The compression type used by the sensor.
        /// </summary>
        public SensorCompressionType CompressionType
        {
            get { return m_CompressionType; }
            set { m_CompressionType = value; }
        }


        /// <summary>
        /// Initializes the sensor.
        /// </summary>
        /// <param name="renderTexture">The [RenderTexture](https://docs.unity3d.com/ScriptReference/RenderTexture.html)
        /// instance to wrap.</param>
        /// <param name="grayscale">Whether to convert it to grayscale or not.</param>
        /// <param name="name">Name of the sensor.</param>
        /// <param name="compressionType">Compression method for the render texture.</param>
        /// [GameObject]: https://docs.unity3d.com/Manual/GameObjects.html
        public SkinSensor(SensingTouch s, Texture2D tex,
            bool grayscale, string name, SensorCompressionType compressionType)
        {
            sensingTouch = s;
            var width = tex != null ? tex.width : 0;
            var height = tex != null ? tex.height : 0;
            m_Grayscale = grayscale;
            m_Name = name;
            m_ObservationSpec = ObservationSpec.Visual(height, width, grayscale ? 1 : 3);
            m_CompressionType = compressionType;
            m_Texture = tex;
            //m_Texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        }

        /// <inheritdoc/>
        public string GetName()
        {
            return m_Name;
        }

        /// <inheritdoc/>
        public ObservationSpec GetObservationSpec()
        {
            using (TimerStack.Instance.Scoped("RenderTextureSensor.Write"))
            {
                return m_ObservationSpec;
            }
        
        }

        /// <inheritdoc/>
        public byte[] GetCompressedObservation()
        {

            using (TimerStack.Instance.Scoped("RenderTextureSensor.Write"))
            {
                //ObservationToTexture(m_RenderTexture, m_Texture);
                // TODO support more types here, e.g. JPG
                var compressed = m_Texture.EncodeToPNG();
                sensingTouch.mainWraper.resetPixels(true);
                Debug.Log("Written");
                return compressed;
            }
        }

        /// <inheritdoc/>
        public int Write(ObservationWriter writer)
        {
          
                
            var numWritten = writer.WriteTexture(m_Texture, m_Grayscale);
            //sensingTouch.mainWraper.resetPixels(true);
            //Debug.Log("Written");
            return numWritten;

        }

        /// <inheritdoc/>
        public void Update() { }

        /// <inheritdoc/>
        public void Reset() { }

        /// <inheritdoc/>
        public CompressionSpec GetCompressionSpec()
        {
            return new CompressionSpec(m_CompressionType);
        }

        /// <inheritdoc/>
        public BuiltInSensorType GetBuiltInSensorType()
        {
            return BuiltInSensorType.RenderTextureSensor;
        }

        /// <summary>
        /// Converts a RenderTexture to a 2D texture.
        /// </summary>
        /// <param name="obsTexture">RenderTexture.</param>
        /// <param name="texture2D">Texture2D to render to.</param>
        public static void ObservationToTexture(RenderTexture obsTexture, Texture2D texture2D)
        {
            var prevActiveRt = RenderTexture.active;
            RenderTexture.active = obsTexture;

            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = prevActiveRt;
        
        }

        /// <summary>
        /// Clean up the owned Texture2D.
        /// </summary>
        public void Dispose()
        {
            if (!ReferenceEquals(null, m_Texture))
            {
                
                m_Texture = null;
            }
        }
    }
}
