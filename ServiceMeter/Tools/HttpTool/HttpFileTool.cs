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

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ServiceMeter;

public partial class HttpTool
{
    public async Task<int> UploadFileAsync(
        string path,
        string fileName,
        byte[] file,
        Dictionary<string, string>? requestHeaders = null,
        string httpFileParameter = "file")
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(file);

        // TODO сделать автоопределение ContentType
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        form.Add(fileContent, httpFileParameter, fileName);

        var response = await this.RequestAsync(
            httpMethod: HttpMethod.Post,
            path: path,
            requestHeaders: requestHeaders,
            requestContent: form);

        return response.StatusCode;
    }

    public async Task<int> UploadFilesAsync(
        string path,
        IEnumerable<(string filename, byte[] fileBytes)> files,
        Dictionary<string, string>? requestHeaders = null,
        string httpFileParameter = "files")
    {
        var form = new MultipartFormDataContent();

        foreach ((string fileName, byte[] fileBytes) in files)
        {
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentDisposition = new("form-data");
            fileContent.Headers.ContentDisposition.Name = httpFileParameter;
            fileContent.Headers.ContentDisposition.FileName = fileName;

            // TODO сделать автоопределение ContentType
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(fileContent);
        }

        HttpResponse response = await this.RequestAsync(
            httpMethod: HttpMethod.Post,
            path: path,
            requestHeaders: requestHeaders,
            requestContent: form);

        return response.StatusCode;
    }

    public async Task<(string?, byte[])> DownloadFileAsync(string path)
    {
        HttpResponse response = await this.RequestAsync(HttpMethod.Get, path);
        string? fileName = response.Filename;
        byte[] bytes = response.Content;

        return (fileName, bytes);
    }
}