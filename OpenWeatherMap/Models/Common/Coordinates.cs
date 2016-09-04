// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Coordinates.cs" company="Joan Caron">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Joan Caron</author>
// <summary>Implements the coordinates class</summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenWeatherMap
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class Coordinates.
    /// </summary>
    public class Coordinates
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the longitude.
        /// </summary>
        /// <value>
        ///     The longitude.
        /// </value>
        [XmlAttribute("lon")]
        public double Longitude { get; set; }

        /// <summary>
        ///     Gets or sets the latitude.
        /// </summary>
        /// <value>
        ///     The latitude.
        /// </value>
        [XmlAttribute("lat")]
        public double Latitude { get; set; }
    }
}
