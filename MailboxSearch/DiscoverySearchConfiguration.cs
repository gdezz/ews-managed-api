// ---------------------------------------------------------------------------
// <copyright file="DiscoverySearchConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------

//-----------------------------------------------------------------------
// <summary>Defines the MailboxQuery class.</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Exchange.WebServices.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents mailbox query object.
    /// </summary>
    public sealed class DiscoverySearchConfiguration
    {
        /// <summary>
        /// Search Id
        /// </summary>
        public string SearchId { get; set; }

        /// <summary>
        /// Search query
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Set of mailbox and scope pair
        /// </summary>
        public SearchableMailbox[] SearchableMailboxes { get; set; }

        /// <summary>
        /// In-Place hold identity
        /// </summary>
        public string InPlaceHoldIdentity { get; set; }

        /// <summary>
        /// Managed by organization
        /// </summary>
        public string ManagedByOrganization { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Load from xml
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>Discovery search configuration object</returns>
        internal static DiscoverySearchConfiguration LoadFromXml(EwsServiceXmlReader reader)
        {
            List<SearchableMailbox> mailboxes = new List<SearchableMailbox>();

            reader.EnsureCurrentNodeIsStartElement(XmlNamespace.Types, XmlElementNames.DiscoverySearchConfiguration);

            DiscoverySearchConfiguration configuration = new DiscoverySearchConfiguration();
            configuration.SearchId = reader.ReadElementValue(XmlNamespace.Types, XmlElementNames.SearchId);

            // the query could be empty means there won't be Query element, hence needs to read and check
            // if the next element is not Query, then it means already read SearchableMailboxes element
            configuration.SearchQuery = string.Empty;
            configuration.InPlaceHoldIdentity = string.Empty;
            configuration.ManagedByOrganization = string.Empty;
            configuration.Language = string.Empty;

            do
            {
                reader.Read();
                if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.SearchQuery))
                {
                    configuration.SearchQuery = reader.ReadElementValue(XmlNamespace.Types, XmlElementNames.SearchQuery);
                    reader.ReadEndElementIfNecessary(XmlNamespace.Types, XmlElementNames.SearchQuery);
                }
                else if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.SearchableMailboxes))
                {
                    // search object without any source mailbox is possible, hence need to check if element is empty
                    if (!reader.IsEmptyElement)
                    {
                        while (!reader.IsEndElement(XmlNamespace.Types, XmlElementNames.SearchableMailboxes))
                        {
                            reader.Read();

                            if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.SearchableMailbox))
                            {
                                mailboxes.Add(SearchableMailbox.LoadFromXml(reader));
                                reader.ReadEndElementIfNecessary(XmlNamespace.Types, XmlElementNames.SearchableMailbox);
                            }
                        }
                    }
                }
                else if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.InPlaceHoldIdentity))
                {
                    configuration.InPlaceHoldIdentity = reader.ReadElementValue(XmlNamespace.Types, XmlElementNames.InPlaceHoldIdentity);
                    reader.ReadEndElementIfNecessary(XmlNamespace.Types, XmlElementNames.InPlaceHoldIdentity);
                }
                else if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.ManagedByOrganization))
                {
                    configuration.ManagedByOrganization = reader.ReadElementValue(XmlNamespace.Types, XmlElementNames.ManagedByOrganization);
                    reader.ReadEndElementIfNecessary(XmlNamespace.Types, XmlElementNames.ManagedByOrganization);
                }
                else if (reader.IsStartElement(XmlNamespace.Types, XmlElementNames.Language))
                {
                    configuration.Language = reader.ReadElementValue(XmlNamespace.Types, XmlElementNames.Language);
                    reader.ReadEndElementIfNecessary(XmlNamespace.Types, XmlElementNames.Language);
                }
                else
                {
                    break;
                }
            }
            while (!reader.IsEndElement(XmlNamespace.Types, XmlElementNames.DiscoverySearchConfiguration));

            configuration.SearchableMailboxes = mailboxes.Count == 0 ? null : mailboxes.ToArray();

            return configuration;
        }

        /// <summary>
        /// Load from json
        /// </summary>
        /// <param name="jsonObject">The json object</param>
        /// <returns>Discovery search configuration object</returns>
        internal static DiscoverySearchConfiguration LoadFromJson(JsonObject jsonObject)
        {
            List<SearchableMailbox> mailboxes = new List<SearchableMailbox>();
            DiscoverySearchConfiguration configuration = new DiscoverySearchConfiguration();

            if (jsonObject.ContainsKey(XmlElementNames.SearchId))
            {
                configuration.SearchId = jsonObject.ReadAsString(XmlElementNames.SearchId);
            }

            if (jsonObject.ContainsKey(XmlElementNames.InPlaceHoldIdentity))
            {
                configuration.InPlaceHoldIdentity = jsonObject.ReadAsString(XmlElementNames.InPlaceHoldIdentity);
            }

            if (jsonObject.ContainsKey(XmlElementNames.ManagedByOrganization))
            {
                configuration.ManagedByOrganization = jsonObject.ReadAsString(XmlElementNames.ManagedByOrganization);
            }

            if (jsonObject.ContainsKey(XmlElementNames.SearchQuery))
            {
                configuration.SearchQuery = jsonObject.ReadAsString(XmlElementNames.SearchQuery);
            }

            if (jsonObject.ContainsKey(XmlElementNames.SearchableMailboxes))
            {
                foreach (object searchableMailboxObject in jsonObject.ReadAsArray(XmlElementNames.SearchableMailboxes))
                {
                    JsonObject jsonSearchableMailbox = searchableMailboxObject as JsonObject;

                    mailboxes.Add(SearchableMailbox.LoadFromJson(jsonSearchableMailbox));
                }
            }

            if (jsonObject.ContainsKey(XmlElementNames.Language))
            {
                configuration.Language = jsonObject.ReadAsString(XmlElementNames.Language);
            }

            configuration.SearchableMailboxes = mailboxes.Count == 0 ? null : mailboxes.ToArray();

            return configuration;
        }
    }
}