import * as WebDataRocksReact from 'react-webdatarocks';
import { Report } from '../webdatarocks';
import { useRef } from 'react';

export interface GridProps {
    innerRef: any;
    data? : any
}

export const gridReportDefaults : Report = {
    dataSource: {
        data: [],
    },options: {
        grid: {
            type: "flat",
            showTotals: "off",
            showGrandTotals: "off",
        },
    },
};

const Grid : React.FC<GridProps> = ({innerRef, data}) => {
    const newReport : Report = {...gridReportDefaults};
    
    return (

            <>
                <table>
                    {}
                </table>
                <WebDataRocksReact.Pivot 
                toolbar={false}
                width="100%"
                report={newReport}
                ref={innerRef}
                />
            </>
        );
}

export default Grid;