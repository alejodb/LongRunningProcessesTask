import "./App.css"
import { TextProcessor } from "./features/textProcessor/TextProcessor"
import "@fontsource/roboto/300.css"
import "@fontsource/roboto/400.css"
import "@fontsource/roboto/500.css"
import "@fontsource/roboto/700.css"
import { Box, Container } from "@mui/material"

export const App = () => (
  <Container maxWidth="sm" className="App">
    <Box sx={{ my: 4 }}>
      <TextProcessor />
    </Box>
  </Container>
)
