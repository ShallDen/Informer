﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sun.cs" company="Joan Caron">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Joan Caron</author>
// <summary>Implements the sun class</summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenWeatherMap
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class Sun.
    /// </summary>
    public sealed class Sun
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the sunrise time.
        /// </summary>
        /// <value>
        ///     The rise.
        /// </value>
        [XmlAttribute("rise")]
        public DateTime Rise { get; set; }

        /// <summary>
        ///     Gets or sets the sunset time.
        /// </summary>
        /// <value>
        ///     The set.
        /// </value>
        [XmlAttribute("set")]
        public DateTime Set { get; set; }
    }
}
