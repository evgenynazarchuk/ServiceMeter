﻿/*
 * MIT License
 *
 * Copyright (c) Evgeny Nazarchuk.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using ServiceMeter.Interfaces;
using ServiceMeter.Reports;
using ServiceMeter.Tools;
using ServiceMeter.Users;

namespace ServiceMeter;

public abstract partial class ChromiumUser : BasicChromiumUser, ISimpleUser, IDisposable
{
    public ChromiumUser(string? userName = null)
        : base(userName ?? typeof(ChromiumUser).Name) { }

    public async Task InvokeAsync(int userLoopCount = 1)
    {
        var pageContext = await this.BrowserTool.GetPageTool();

        //pageContext.Page.RequestFinished += (_, request) =>
        //{
        //    if (this.Watcher is not null)
        //    {
        //        this.Watcher.SendMessage("PageRequestLog.json",
        //            $"{this.UserName}\t" +
        //            $"{request.Method}\t" +
        //            $"{request.Url}\t" + // how to parse url, error parse csv
        //            $"{TimeSpan.FromMilliseconds(request.Timing.RequestStart)}\t" +
        //            $"{TimeSpan.FromMilliseconds(request.Timing.ResponseStart)}\t" +
        //            $"{TimeSpan.FromMilliseconds(request.Timing.ResponseEnd)}",
        //            typeof(ChromiumPageRequestLogMessage));
        //    }
        //};

        for (int i = 0; i < userLoopCount; i++)
        {
            await Performance(pageContext);
        }

        await pageContext.CloseAsync();
    }

    protected abstract Task Performance(PageTool pageContext);
}
