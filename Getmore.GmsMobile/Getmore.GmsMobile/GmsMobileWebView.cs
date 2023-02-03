using Getmore.GmsMobile.Core.GmsInterface;
using Getmore.GmsMobile.Core.Messages;
using Json.Net;
using System;
using Xamarin.Forms;

namespace Getmore.GmsMobile
{
	public class GmsMobileWebView : WebView
	{
		public void Cleanup()
		{
			this._displayJSTextAction = null;
		}

		#region DisplayJSTextAction

		private Action<string> _displayJSTextAction;

		public void RegisterDisplayJSTextAction(Action<string> callback)
		{
			this._displayJSTextAction = callback;
		}

		public void InvokeDisplayJSTextAction(string text)
		{
			this._displayJSTextAction?.Invoke(text);
		}

		#endregion


		#region IncommingMessageAction

		private Action<MessageType, MessageBase> _incommingMessageAction;

		public void RegisterIncommingMessageAction(Action<MessageType, MessageBase> callback)
		{
			this._incommingMessageAction = callback;
		}

		public void InvokeIncommingMessageAction(string messageTypeString, string messageJSON)
		{
			if (!Enum.TryParse(messageTypeString, true, out MessageType messageType))
				throw new InvalidOperationException($"Unable to parse MessageType from {messageTypeString}...");

			var message = String.IsNullOrWhiteSpace(messageJSON)
				? null
				: messageType switch
				{
					MessageType.NewPushNotification => JsonNet.Deserialize<NewPushNotificationMessage>(messageJSON),
					MessageType.Restart => throw new NotImplementedException(),
					_ => throw new InvalidOperationException($"Unsupported message type {messageType}...")
				} as MessageBase;

			this._incommingMessageAction?.Invoke(messageType, message);
		}

		#endregion

		public void SendCommandToGmsApp(GmsAppCommandType command, object commandArgs)
		{
			Device.BeginInvokeOnMainThread(async () => await this.EvaluateJavaScriptAsync($"SendCommandToGmsApp('{command}', {JsonNet.Serialize(commandArgs)})")); // 
		}
	}
}
