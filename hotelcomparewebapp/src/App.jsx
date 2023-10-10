import React, { Component } from 'react';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = { results: [], loading: true };
    }

    componentDidMount() {
        this.getReviewData();
    }

    static renderForecastsTable(results) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Conclusion</th>
                        <th>Score</th>
                        <th>Reason</th>
                    </tr>
                </thead>
                <tbody>
                    {results.map(result =>
                        <tr key={result.Conclusion}>
                            <td>{result.Score}</td>
                            <td>{result.Reason}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
            : App.renderForecastsTable(this.state.results);

        return (
            <div>
                <h1 id="tabelLabel" >Weather forecast</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }

    async getReviewData() {
        fetch("https://localhost:7133/chat")
            .then(res => res.json())
            .then(
                (data) => this.setState({ results: data, loading: false })
            )
    }
}
