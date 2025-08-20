import { HubConnectionBuilder } from "@microsoft/signalr"
import type { ReactElement } from "react"
import { useEffect, useRef, useState } from "react"
import {
  useCountTextOccurrencesMutation,
  useCancelProcessMutation,
} from "./textProcessorApiSlice"
import Button from "@mui/material/Button"
import { Alert, Box, Divider, LinearProgress, Stack, TextField, Typography } from "@mui/material"

export const TextProcessor = (): ReactElement => {
  const [message, setMessage] = useState<string>("")
  const [connectionId, setConnectionId] = useState<string | null>(null)
  const [responseMessage, setResponseMessage] = useState<string>("")
  const [isProcessingMessage, setIsProcessingMessage] = useState<boolean>(false)
  const [statusMessage, setStatusMessage] = useState<string | undefined>()

  const [
    sendMessage,
    {
      isError: isSendMessageError,
      isLoading: isSendMessageLoading,
      data: countTextOcurrencesResponse,
    },
  ] = useCountTextOccurrencesMutation()

  const [
    cancelProcess,
    { isError: isCancelProcessError, isLoading: isCancelProcessLoading },
  ] = useCancelProcessMutation()

  const initialized = useRef(false)

  useEffect(() => {
    if (!initialized.current) {
      console.log("Setting up SignalR connection...")
      const setupConnection = async () => {
        const connection = new HubConnectionBuilder()
          .withUrl("http://localhost:5281/chatHub")
          .build()

        await connection.start()
        setConnectionId(connection.connectionId)
        connection.on("ReceiveMessage", (text: string) => {
          console.log("New message received:", text)
          setResponseMessage(responseMessage => responseMessage + text)
        })
        connection.on("StatusMessage", (message: string) => {
          setIsProcessingMessage(false)
          setStatusMessage(message)
        })
      }

      setupConnection().then(
        () => {
          console.log("Success")
        },
        () => {
          console.log("Error connecting to SignalR")
        },
      )

      initialized.current = true
    }
  }, [])

  return (
    <Stack spacing={2} sx={{ width: "100%" }}>
      <Typography variant="h4">LONG-RUNNING PROCESS TASK</Typography>
      <Divider />
      <Typography variant="h6">Enter the text you want to process:</Typography>

      <TextField
        value={message}
        onChange={e => {
          setMessage(e.target.value)
        }}
        variant="outlined"
        fullWidth
      />
      <Box sx={{ display: "flex", gap: 2 }}>
        <Button
          disabled={isSendMessageLoading || isProcessingMessage || !connectionId}
          variant="contained"
          color="primary"
          onClick={() => {
            setResponseMessage("")
            setStatusMessage(undefined)
            sendMessage({ text: message, connectionId: connectionId }).then(
              () => {
                setIsProcessingMessage(true)
              },
              (error: unknown) => {
                console.error("Error sending message:", error)
              },
            )
          }}
        >
          Send Message
        </Button>
        <Button
          disabled={!isProcessingMessage || isCancelProcessLoading || !countTextOcurrencesResponse?.processId}
          variant="outlined"
          color="secondary"
          onClick={() => {
            if (countTextOcurrencesResponse?.processId) {
              setStatusMessage(undefined)
              cancelProcess(countTextOcurrencesResponse.processId).then(undefined, (error: unknown) => {
                console.error("Error canceling process:", error)
              })
            }
            // TODO: how to manage the cancelation action when no process in progress
          }}
        >
          Cancel
        </Button>
      </Box>

      <LinearProgress
        sx={{ display: isProcessingMessage ? "flex" : "none" }}
      />

      <Typography variant="h6">Result:</Typography>
      <Typography variant="body1">{responseMessage}</Typography>
      <Alert
        severity="error"
        sx={{ display: isSendMessageError ? "flex" : "none" }}
      >
        {isSendMessageError && "Error sending message"}
      </Alert>
      <Alert
        severity="error"
        sx={{ display: isCancelProcessError ? "flex" : "none" }}
      >
        {isCancelProcessError && "Error canceling process"}
      </Alert>
      <Alert severity="info" sx={{ display: statusMessage ? "flex" : "none" }}>
        {statusMessage}
      </Alert>
    </Stack>
  )
}
