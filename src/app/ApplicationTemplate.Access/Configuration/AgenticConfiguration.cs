namespace ApplicationTemplate.DataAccess.Configuration;

/// <summary>
/// Configuration for Agentic (Azure AI Foundry).
/// </summary>
public class AgenticConfiguration
{
	/// <summary>
	/// Gets or sets the Azure AI Foundry endpoint URL.
	/// </summary>
	public string Endpoint { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure AI Foundry API key.
	/// </summary>
	public string ApiKey { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the deployment name or model name.
	/// </summary>
	public string DeploymentName { get; set; } = "gpt-4";

	/// <summary>
	/// Gets or sets the temperature for the AI model (0.0 - 1.0).
	/// </summary>
	public double Temperature { get; set; } = 0.7;

	/// <summary>
	/// Gets or sets the maximum number of tokens in the response.
	/// </summary>
	public int MaxTokens { get; set; } = 1000;

	/// <summary>
	/// Gets or sets the Azure Speech Services endpoint URL.
	/// </summary>
	public string SpeechEndpoint { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure Speech Services API key.
	/// </summary>
	public string SpeechApiKey { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a value indicating whether voice input is enabled.
	/// </summary>
	public bool VoiceInputEnabled { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether voice output is enabled.
	/// </summary>
	public bool VoiceOutputEnabled { get; set; } = true;
}
