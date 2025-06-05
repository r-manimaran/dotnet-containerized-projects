import http from 'k6/http';
import { check } from 'k6';

export let options = {
    scenarios: {
        yarp: {
            executor: 'per-vu-iterations',
            exec: 'yarp',
            iterations: 1000, // Total number of iterations
            startTime: '0s', // Start time of the test
            vus: 200, // Number of virtual users
           // duration: '30s', // Duration of the test
        },
    }
};
export function yarp() {
    let res = http.get("http://localhost:3000/hello");
    check(res, {
        "YARP: status 200": (r) => r.status === 200,
    });
}
