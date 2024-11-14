// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Agents.Protocols.Primitives;
using Microsoft.Agents.Protocols.Serializer;

namespace Microsoft.Agents.Memory.Transcript
{
    /// <summary>
    /// Represents a transcript logger that writes activities to a Trace object.
    /// </summary>
    public class TraceTranscriptLogger : ITranscriptLogger
    {
        private static readonly JsonSerializerOptions _serializationSettings = ProtocolJsonSerializer.SerializationOptions;

        private readonly bool _traceActivity;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTranscriptLogger"/> class.
        /// </summary>
        public TraceTranscriptLogger()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTranscriptLogger"/> class.
        /// </summary>
        /// <param name="traceActivity">Indicates if trace information should be logged.</param>
        public TraceTranscriptLogger(bool traceActivity)
        {
            this._traceActivity = traceActivity;
        }

        /// <summary>
        /// Log an activity to the transcript.
        /// </summary>
        /// <param name="activity">The activity to transcribe.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public Task LogActivityAsync(IActivity activity)
        {
            ArgumentNullException.ThrowIfNull(activity);

            if (_traceActivity)
            {
                System.Diagnostics.Trace.TraceInformation(ProtocolJsonSerializer.ToJson(activity));
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached && activity.Type == ActivityTypes.Message)
                {
                    System.Diagnostics.Trace.TraceInformation($"{activity.From.Name ?? activity.From.Id ?? activity.From.Role} [{activity.Type}] {activity.Text}");
                }
                else
                {
                    System.Diagnostics.Trace.TraceInformation($"{activity.From.Name ?? activity.From.Id ?? activity.From.Role} [{activity.Type}]");
                }
            }

            return Task.CompletedTask;
        }
    }
}
