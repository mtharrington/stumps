﻿namespace Stumps.Rules
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the HTTP method of an HTTP request.
    /// </summary>
    public class HttpMethodRule : IStumpRule
    {
        private const string HttpMethodSetting = "httpmethod.value";

        private string _textMatchValue;
        private TextMatch _textMatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpMethodRule"/> class.
        /// </summary>
        public HttpMethodRule()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpMethodRule"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the rule.</param>
        public HttpMethodRule(string httpMethod)
        {
            InitializeRule(httpMethod);
        }

        /// <summary>
        ///     Gets the text match rule for the HTTP method.
        /// </summary>
        /// <value>
        ///     The text match rule for the HTTP method.
        /// </value>
        public string HttpMethodTextMatch
        {
            get => _textMatchValue;
        }
        
        /// <summary>
        ///     Gets a value indicating whether the rule is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the rule is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets an enumerable list of <see cref="RuleSetting" /> objects used to represent the current instance.
        /// </summary>
        /// <returns>
        ///     An enumerable list of <see cref="RuleSetting" /> objects used to represent the current instance.
        /// </returns>
        public IEnumerable<RuleSetting> GetRuleSettings()
        {
            var settings = new[]
            {
                new RuleSetting
                {
                    Name = HttpMethodRule.HttpMethodSetting,
                    Value = _textMatchValue
                }
            };

            return settings;
        }

        /// <summary>
        ///     Initializes a rule from an enumerable list of <see cref="RuleSetting" /> objects.
        /// </summary>
        /// <param name="settings">The enumerable list of <see cref="RuleSetting" /> objects.</param>
        public void InitializeFromSettings(IEnumerable<RuleSetting> settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (this.IsInitialized)
            {
                throw new InvalidOperationException(BaseResources.BodyRuleAlreadyInitializedError);
            }

            var helper = new RuleSettingsHelper(settings);
            var httpMethod = helper.FindString(HttpMethodRule.HttpMethodSetting, string.Empty);

            InitializeRule(httpMethod);
        }

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="IStumpsHttpRequest" /> to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if the <paramref name="request" /> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpRequest request)
        {
            if (request == null)
            {
                return false;
            }

            var match = _textMatch.IsMatch(request.HttpMethod);
            return match;
        }

        /// <summary>
        ///     Initializes the rule.
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the rule.</param>
        public void InitializeRule(string httpMethod)
        {
            _textMatchValue = httpMethod ?? string.Empty;
            _textMatch = new TextMatch(_textMatchValue, true);
            this.IsInitialized = true;
        }
    }
}