import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import type { LinearProgressProps } from '@mui/material';
import { LinearProgress } from '@mui/material';
import type { ReactElement } from 'react';

export const LinearProgressWithLabel = (props: LinearProgressProps & { value: number }): ReactElement => {
  return (
    <Box sx={{ display: 'flex', alignItems: 'center' }}>
      <Box sx={{ width: '100%', mr: 1 }}>
        <LinearProgress variant="determinate" {...props} />
      </Box>
      <Box sx={{ minWidth: 35 }}>
        <Typography
          variant="body2"
          sx={{ color: 'text.secondary' }}
        >{`${Math.round(props.value).toString()}%`}</Typography>
      </Box>
    </Box>
  );
}