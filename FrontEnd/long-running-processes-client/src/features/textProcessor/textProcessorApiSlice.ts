import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react"
import { API_URL } from '../../config'

type CountTextOcurrencesRequest = {
  text: string
  connectionId: string | null
}

type CountTextOcurrencesResponse = {
  processId: string
}

export const textProcessorApiSlice = createApi({
  baseQuery: fetchBaseQuery({ baseUrl: API_URL }),
  reducerPath: "textProcessorApi",
  endpoints: (builder) => ({
    countTextOccurrences: builder.mutation<CountTextOcurrencesResponse, CountTextOcurrencesRequest>({
      query: ({ text, connectionId }) => {
        return {
          url: "/textprocessor",
          method: "POST",
          body: {
            text,
            connectionId
          }
        }
      },
    }),
    cancelProcess: builder.mutation<undefined, string>({
      query: (processId) => {
        return {
          url: `/textprocessor/${processId}`,
          method: "DELETE"
        }
      },
    }),
  }),
})

export const { useCountTextOccurrencesMutation, useCancelProcessMutation } = textProcessorApiSlice
