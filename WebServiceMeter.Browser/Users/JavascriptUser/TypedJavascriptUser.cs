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

using System.Threading.Tasks;
using WebServiceMeter.Interfaces;
using WebServiceMeter.Users;

namespace WebServiceMeter;

public abstract partial class JavascriptUser<TData> : BasicJavascriptUser, ITypedUser<TData>
        where TData : class
{
    public JavascriptUser(string? userName = null)
        : base(userName ?? typeof(JavascriptUser).Name) { }

    public async Task InvokeAsync(
        IDataReader<TData> dataReader,
        bool reuseDataInLoop = true,
        int userloopCount = 1)
    {
        ////this.Page.RequestFinished += (_, request) =>
        ////{
        ////    Console.WriteLine($"{TimeSpan.FromMilliseconds(request.Timing.ConnectStart)} " +
        ////        $"{TimeSpan.FromMilliseconds(request.Timing.ConnectEnd)} " +
        ////        $"{TimeSpan.FromMilliseconds(request.Timing.RequestStart)} " +
        ////        $"{TimeSpan.FromMilliseconds(request.Timing.ResponseStart)} " +
        ////        $"{TimeSpan.FromMilliseconds(request.Timing.ResponseEnd)} " +
        ////        $"{request.Method} " +
        ////        $"{request.Url}");
        ////};

        TData? data = dataReader.GetData();

        for (int i = 0; i < userloopCount; i++)
        {
            if (data is null)
            {
                continue;
            }

            await PerformanceAsync(data);

            if (!reuseDataInLoop)
            {
                data = dataReader.GetData();
            }
        }
    }

    protected abstract Task PerformanceAsync(TData data);
}