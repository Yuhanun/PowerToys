﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Plugin.Program.Storage
{
    public static class EventHandler
    {
        // To obtain the path of the app when multiple events are added to the Concurrent queue across multiple threads.
        // On the first occurence of a different file path, the existing app path is to be returned without removing any more elements from the queue.
        public static string GetAppPathFromQueue(ConcurrentQueue<string> eventHandlingQueue, int dequeueDelay)
        {
            if (eventHandlingQueue == null)
            {
                throw new ArgumentNullException(nameof(eventHandlingQueue));
            }

            string previousAppPath = string.Empty;

            // To obtain the last event associated with a particular app.
            while (eventHandlingQueue.TryPeek(out string currentAppPath))
            {
                if (string.IsNullOrEmpty(previousAppPath) || previousAppPath.Equals(currentAppPath, StringComparison.OrdinalIgnoreCase))
                {
                    // To dequeue a path only if it is the first one in the queue or if the path was the same as thre previous one (to avoid trying to create apps on duplicate events)
                    previousAppPath = currentAppPath;
                    eventHandlingQueue.TryDequeue(out _);
                }
                else
                {
                    break;
                }

                // This delay has been added to account for the delay in events being triggered during app installation.
                Thread.Sleep(dequeueDelay);
            }

            return previousAppPath;
        }
    }
}
