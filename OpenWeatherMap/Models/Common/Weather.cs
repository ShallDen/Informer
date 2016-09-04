
namespace OpenWeatherMap
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class Weather.
    /// </summary>
    public sealed class Weather
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the number.
        /// </summary>
        /// <value>
        ///     The number.
        /// </value>
        [XmlAttribute("number")]
        public int Number { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [XmlAttribute("value")]
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the icon filename without extension.
        /// </summary>
        /// <value>
        ///     The icon.
        /// </value>
        [XmlAttribute("icon")]
        public string Icon { get; set; }
    }
}
