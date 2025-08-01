using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TeraCyteAssignment.Models
{
    public record LoginRequest(
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("password")] string Password
    );

    public record LoginResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken
    );

    public record RefreshRequest(
        [property: JsonPropertyName("refresh_token")] string RefreshToken
    );

    public record ImageResponse(
        [property: JsonPropertyName("image_id")] string ImageId,
        [property: JsonPropertyName("image_data_base64")] string Base64ImageData
    );

    public record ResultsResponse(
        [property: JsonPropertyName("image_id")] string ImageId,
        [property: JsonPropertyName("intensity_average")] float IntensityAverage,
        [property: JsonPropertyName("focus_score")] float FocusScore,
        [property: JsonPropertyName("classification_label")] string ClassificationLabel,
        [property: JsonPropertyName("histogram")] int[] Histogram
    );

    public record InferenceData(
        string ImageId,
        string Base64Image,
        string ClassificationLabel,
        float FocusScore,
        float IntensityAverage,
        int[] Histogram
    );

    public record HistogramBar(double Left, double Height, double Width);
}



