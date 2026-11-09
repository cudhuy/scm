'use client';

import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { FileDown } from 'lucide-react';

interface ExportButtonsProps {
  exportCSV: () => Promise<string>;
  exportJSON: () => Promise<string>;
  fileNamePrefix?: string;
  isDisabled?: boolean;
}

export function ExportButtons({
  exportCSV,
  exportJSON,
  fileNamePrefix = 'data',
  isDisabled = false,
}: ExportButtonsProps) {
  const handleExportCSV = async () => {
    try {
      const csvData = await exportCSV();
      downloadFile(csvData, `${fileNamePrefix}.csv`, 'text/csv');
    } catch (error) {
      console.error('Error exporting CSV:', error);
    }
  };

  const handleExportJSON = async () => {
    try {
      const jsonData = await exportJSON();
      downloadFile(jsonData, `${fileNamePrefix}.json`, 'application/json');
    } catch (error) {
      console.error('Error exporting JSON:', error);
    }
  };

  const downloadFile = (data: string, fileName: string, mimeType: string) => {
    const blob = new Blob([data], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="outline" disabled={isDisabled}>
          <FileDown className="h-4 w-4 mr-2" />
          <span>Xuất</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuItem onClick={handleExportCSV}>Xuất sang CSV</DropdownMenuItem>
        <DropdownMenuItem onClick={handleExportJSON}>Xuất sang JSON</DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
