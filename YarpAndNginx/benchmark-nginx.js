import http from 'k6/http';
import { check } from 'k6';

export let options = {
    scenarios: {
        nginx: {
            executor: 'per-vu-iterations',
            exec: 'testNginx',
            iterations: 1000, // Total number of iterations
            startTime: '0s', // Start time of the test
            vus: 200, // Number of virtual users
            // duration: '30s', // Duration of the test
        },
    }
};
export function testNginx() {
    let res = http.get("http://localhost:3001/hello");
    check(res, {
        "YARP: status 200": (r) => r.status === 200,
    });
}
