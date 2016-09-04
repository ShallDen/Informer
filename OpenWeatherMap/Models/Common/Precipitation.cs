// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Precipitation.cs" company="Joan Caron">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Joan Caron</author>
// <summary>Implements the precipitation class</summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenWeatherMap
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class Precipitation.
    /// </summary>
    public sealed class Precipitation
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [XmlAttribute("value")]
        public double Value { get; set; }

        /// <summary>
        ///     Gets or sets the mode.
        /// </summary>
        /// <value>
        ///     The mode.
        /// </value>
        [XmlAttribute("mode")]
        public string Mode { get; set; }

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        /// <value>
        ///     The unit.
        /// </value>
        [XmlAttribute("unit")]
        public string Unit { get; set; }
    }
}
