namespace ApplicationTemplate.DataAccess.Configuration;

/// <summary>
/// Configuration for Agentic (Azure AI Foundry Agents).
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
	/// Gets or sets the Azure subscription ID (required for connection string).
	/// </summary>
	public string SubscriptionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure AD Tenant ID for Service Principal authentication.
	/// </summary>
	public string TenantId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure AD Client ID (Application ID) for Service Principal authentication.
	/// </summary>
	public string ClientId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure AD Client Secret for Service Principal authentication.
	/// </summary>
	public string ClientSecret { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure resource group name (required for connection string).
	/// </summary>
	public string ResourceGroup { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the Azure resource name (required for connection string).
	/// </summary>
	public string ResourceName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the deployment name or model name.
	/// </summary>
	public string DeploymentName { get; set; }


	/// <summary>
	/// Gets or sets the temperature for the AI model (0.0 - 1.0).
	/// </summary>
	public double Temperature { get; set; } = 0.7;

	/// <summary>
	/// Gets or sets the maximum number of tokens in the response.
	/// </summary>
	public int MaxTokens { get; set; } = 1000;

	/// <summary>
	/// Gets or sets a value indicating whether voice input is enabled (uses GPT-4o multimodal audio).
	/// </summary>
	public bool VoiceInputEnabled { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether voice output is enabled (uses GPT-4o multimodal audio).
	/// </summary>
	public bool VoiceOutputEnabled { get; set; } = true;

	/// <summary>
	/// Gets or sets the audio format for multimodal audio input/output (wav, mp3, etc.).
	/// </summary>
	public string AudioFormat { get; set; } = "wav";

	/// <summary>
	/// Gets or sets the voice to use for audio output (alloy, echo, fable, onyx, nova, shimmer).
	/// </summary>
	public string Voice { get; set; } = "alloy";

	/// <summary>
	/// Gets or sets the assistant name to search for or create in Azure AI Foundry.
	/// The application will search for an existing assistant with this name and use it if found,
	/// or create a new one if not found.
	/// </summary>
	public string AssistantName { get; set; } = "Mobile App Assistant";

	/// <summary>
	/// Gets or sets the assistant instructions (system prompt).
	/// Can be a direct string or a file path (relative to the app directory) ending in .md or .txt.
	/// </summary>
	public string AssistantInstructions { get; set; } = "AssistantInstructions.md";
}
